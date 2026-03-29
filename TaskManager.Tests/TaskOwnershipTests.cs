using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Models;
using TaskManager.Api.Repositories;
using TaskManager.Api.Services;
using Xunit;

namespace TaskManager.Tests
{
    public class TaskOwnershipTests
    {
        private TaskDbContext GetDbContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<TaskDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new TaskDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private async Task<User> CreateTestUser(TaskDbContext context, string email = "user@test.com", string username = "testuser")
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                PasswordHash = "hashed_password",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        #region Create with UserId Tests
        [Fact]
        public async Task CreateAsync_WithUserId_SetsUserIdOnTask()
        {
            // Arrange
            var context = GetDbContext();
            var user = await CreateTestUser(context);
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            var task = new TaskItem
            {
                Title = "User Task",
                UserId = user.Id
            };

            // Act
            var created = await service.CreateAsync(task);

            // Assert
            created.UserId.Should().Be(user.Id);
            created.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task CreateAsync_WithoutUserId_ThrowsArgumentException()
        {
            // Arrange
            var context = GetDbContext();
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            var task = new TaskItem
            {
                Title = "Task Without User",
                UserId = Guid.Empty  // Invalid
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(task));
        }

        #endregion

        #region GetAll with Filtering Tests
        [Fact]
        public async Task GetAllAsync_WithUserId_ReturnsOnlyUserTasks()
        {
            // Arrange
            var context = GetDbContext();
            var user1 = await CreateTestUser(context, "user1@test.com", "user1");
            var user2 = await CreateTestUser(context, "user2@test.com", "user2");
            
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            // Create tasks for user1
            var task1 = new TaskItem { Title = "User1 Task 1", UserId = user1.Id };
            var task2 = new TaskItem { Title = "User1 Task 2", UserId = user1.Id };
            await service.CreateAsync(task1);
            await service.CreateAsync(task2);

            // Create tasks for user2
            var task3 = new TaskItem { Title = "User2 Task 1", UserId = user2.Id };
            await service.CreateAsync(task3);

            // Act
            var user1Tasks = await service.GetAllAsync(user1.Id);
            var user2Tasks = await service.GetAllAsync(user2.Id);

            // Assert
            user1Tasks.Should().HaveCount(2);
            user1Tasks.All(t => t.UserId == user1.Id).Should().BeTrue();
            
            user2Tasks.Should().HaveCount(1);
            user2Tasks.All(t => t.UserId == user2.Id).Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_WithUserId_ReturnsEmptyWhenNoTasks()
        {
            // Arrange
            var context = GetDbContext();
            var user = await CreateTestUser(context);
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            // Act
            var tasks = await service.GetAllAsync(user.Id);

            // Assert
            tasks.Should().BeEmpty();
        }

        #endregion

        #region GetById with Ownership Validation Tests
        [Fact]
        public async Task GetByIdAsync_WithCorrectOwner_ReturnsTask()
        {
            // Arrange
            var context = GetDbContext();
            var owner = await CreateTestUser(context, "owner@test.com", "owner");
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            var task = new TaskItem { Title = "Owner Task", UserId = owner.Id };
            var created = await service.CreateAsync(task);

            // Act
            var found = await service.GetByIdAsync(created.Id, owner.Id);

            // Assert
            found.Should().NotBeNull();
            found!.Id.Should().Be(created.Id);
            found.UserId.Should().Be(owner.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithWrongOwner_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = GetDbContext();
            var owner = await CreateTestUser(context, "owner@test.com", "owner");
            var intruder = await CreateTestUser(context, "intruder@test.com", "intruder");
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            var task = new TaskItem { Title = "Owner Task", UserId = owner.Id };
            var created = await service.CreateAsync(task);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                service.GetByIdAsync(created.Id, intruder.Id));
        }

        #endregion

        #region Update with Ownership Validation Tests
        [Fact]
        public async Task UpdateAsync_WithCorrectOwner_UpdatesTask()
        {
            // Arrange
            var context = GetDbContext();
            var owner = await CreateTestUser(context, "owner@test.com", "owner");
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            var task = new TaskItem { Title = "Original", UserId = owner.Id };
            var created = await service.CreateAsync(task);

            var update = new TaskItem { Title = "Updated", UserId = owner.Id };

            // Act
            var updated = await service.UpdateAsync(created.Id, update, owner.Id);

            // Assert
            updated.Title.Should().Be("Updated");
            updated.UserId.Should().Be(owner.Id);
        }

        [Fact]
        public async Task UpdateAsync_WithWrongOwner_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = GetDbContext();
            var owner = await CreateTestUser(context, "owner@test.com", "owner");
            var intruder = await CreateTestUser(context, "intruder@test.com", "intruder");
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            var task = new TaskItem { Title = "Owner Task", UserId = owner.Id };
            var created = await service.CreateAsync(task);

            var update = new TaskItem { Title = "Hacked", UserId = owner.Id };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                service.UpdateAsync(created.Id, update, intruder.Id));
        }

        #endregion

        #region Delete with Ownership Validation Tests
        [Fact]
        public async Task DeleteAsync_WithCorrectOwner_DeletesTask()
        {
            // Arrange
            var context = GetDbContext();
            var owner = await CreateTestUser(context, "owner@test.com", "owner");
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            var task = new TaskItem { Title = "To Delete", UserId = owner.Id };
            var created = await service.CreateAsync(task);

            // Act
            await service.DeleteAsync(created.Id, owner.Id);

            // Assert
            var found = await context.Tasks.FirstOrDefaultAsync(t => t.Id == created.Id);
            found.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_WithWrongOwner_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = GetDbContext();
            var owner = await CreateTestUser(context, "owner@test.com", "owner");
            var intruder = await CreateTestUser(context, "intruder@test.com", "intruder");
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            var task = new TaskItem { Title = "Protected Task", UserId = owner.Id };
            var created = await service.CreateAsync(task);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                service.DeleteAsync(created.Id, intruder.Id));
        }

        #endregion

        #region SetStatus with Ownership Validation Tests
        [Fact]
        public async Task SetStatusAsync_WithCorrectOwner_ChangesStatus()
        {
            // Arrange
            var context = GetDbContext();
            var owner = await CreateTestUser(context, "owner@test.com", "owner");
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            var task = new TaskItem { Title = "Status Task", Status = TaskItemStatus.Todo, UserId = owner.Id };
            var created = await service.CreateAsync(task);

            // Act
            var updated = await service.SetStatusAsync(created.Id, TaskItemStatus.InProgress, owner.Id);

            // Assert
            updated.Status.Should().Be(TaskItemStatus.InProgress);
            updated.UserId.Should().Be(owner.Id);
        }

        [Fact]
        public async Task SetStatusAsync_WithWrongOwner_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = GetDbContext();
            var owner = await CreateTestUser(context, "owner@test.com", "owner");
            var intruder = await CreateTestUser(context, "intruder@test.com", "intruder");
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            var task = new TaskItem { Title = "Protected Task", Status = TaskItemStatus.Todo, UserId = owner.Id };
            var created = await service.CreateAsync(task);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                service.SetStatusAsync(created.Id, TaskItemStatus.Done, intruder.Id));
        }

        #endregion

        #region Multi-User Isolation Tests
        [Fact]
        public async Task MultipleUsers_TasksAreIsolated()
        {
            // Arrange
            var context = GetDbContext();
            var user1 = await CreateTestUser(context, "user1@test.com", "user1");
            var user2 = await CreateTestUser(context, "user2@test.com", "user2");
            var user3 = await CreateTestUser(context, "user3@test.com", "user3");

            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository);

            // Create multiple tasks for each user
            var tasks1 = new List<TaskItem>();
            for (int i = 1; i <= 3; i++)
                tasks1.Add(await service.CreateAsync(new TaskItem { Title = $"User1 Task {i}", UserId = user1.Id }));

            var tasks2 = new List<TaskItem>();
            for (int i = 1; i <= 2; i++)
                tasks2.Add(await service.CreateAsync(new TaskItem { Title = $"User2 Task {i}", UserId = user2.Id }));

            var tasks3 = new List<TaskItem>();
            for (int i = 1; i <= 1; i++)
                tasks3.Add(await service.CreateAsync(new TaskItem { Title = $"User3 Task {i}", UserId = user3.Id }));

            // Act & Assert
            var user1All = await service.GetAllAsync(user1.Id);
            var user2All = await service.GetAllAsync(user2.Id);
            var user3All = await service.GetAllAsync(user3.Id);

            user1All.Should().HaveCount(3);
            user2All.Should().HaveCount(2);
            user3All.Should().HaveCount(1);

            // User2 cannot access User1's tasks
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                service.GetByIdAsync(tasks1[0].Id, user2.Id));

            // User3 cannot modify User1's tasks
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                service.SetStatusAsync(tasks1[0].Id, TaskItemStatus.Done, user3.Id));
        }

        #endregion
    }
}
