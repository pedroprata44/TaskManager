using System;
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
    public class TaskServiceTests
    {
        private readonly Guid _defaultUserId = Guid.NewGuid();

        [Fact]
        public async Task GetAll_WhenNoTasks_ReturnsEmptyCollection()
        {
            var repository = GetRepository();
            var service = new TaskService(repository);

            var tasks = await service.GetAllAsync();

            tasks.Should().BeEmpty();
        }

        [Fact]
        public async Task GetById_WhenTaskExists_ReturnsTask()
        {
            var repository = GetRepository();
            var service = new TaskService(repository);

            var task = new TaskItem { Title = "Existente", UserId = _defaultUserId };
            var created = await service.CreateAsync(task);

            var found = await service.GetByIdAsync(created.Id);

            found.Should().NotBeNull();
            found!.Id.Should().Be(created.Id);
            found.Title.Should().Be("Existente");
        }

        [Fact]
        public async Task GetById_WhenTaskDoesNotExist_ReturnsNull()
        {
            var repository = GetRepository();
            var service = new TaskService(repository);

            var found = await service.GetByIdAsync(Guid.NewGuid());

            found.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidTask_ReturnsTaskWithIdAndCreatedAt()
        {
            var repository = new InMemoryTaskRepository();
            var service = new TaskService(repository);

            var task = new TaskItem
            {
                Title = "Teste TDD",
                Description = "Escrever primeiro teste",
                DueDate = DateTime.UtcNow.AddDays(3),
                Priority = TaskItemPriority.High,
                UserId = _defaultUserId
            };

            var created = await service.CreateAsync(task);

            created.Id.Should().NotBe(Guid.Empty);
            created.Title.Should().Be("Teste TDD");
            created.Status.Should().Be(TaskItemStatus.Todo);
            created.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));

            var all = (await service.GetAllAsync()).ToList();
            all.Should().ContainSingle().Which.Id.Should().Be(created.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateAsync_InvalidTitle_ThrowsArgumentException(string invalidTitle)
        {
            var repository = GetRepository();
            var service = new TaskService(repository);

            var task = new TaskItem { Title = invalidTitle, UserId = _defaultUserId };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(task));
        }

        [Fact]
        public async Task UpdateAsync_ExistingTask_UpdatesFields()
        {
            var repository = GetRepository();
            var service = new TaskService(repository);

            var original = new TaskItem
            {
                Title = "Original",
                Description = "Desc",
                DueDate = DateTime.UtcNow.AddDays(2),
                Priority = TaskItemPriority.Low,
                UserId = _defaultUserId
            };

            var created = await service.CreateAsync(original);

            var dueDate = DateTime.UtcNow.AddDays(5);
            var update = new TaskItem
            {
                Title = "Updated",
                Description = "Updated Desc",
                DueDate = dueDate,
                Priority = TaskItemPriority.High,
                Status = TaskItemStatus.InProgress,
                UserId = _defaultUserId
            };

            var updated = await service.UpdateAsync(created.Id, update);

            updated.Id.Should().Be(created.Id);
            updated.Title.Should().Be("Updated");
            updated.Description.Should().Be("Updated Desc");
            updated.DueDate.Should().BeCloseTo(dueDate, TimeSpan.FromSeconds(1));
            updated.Priority.Should().Be(TaskItemPriority.High);
            updated.Status.Should().Be(TaskItemStatus.InProgress);
        }

        [Fact]
        public async Task UpdateAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            var repository = GetRepository();
            var service = new TaskService(repository);

            var task = new TaskItem { Title = "DoesNotMatter", UserId = _defaultUserId };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(Guid.NewGuid(), task));
        }

        [Fact]
        public async Task UpdateAsync_InvalidTitle_ThrowsArgumentException()
        {
            var repository = GetRepository();
            var service = new TaskService(repository);

            var created = await service.CreateAsync(new TaskItem { Title = "Valid", UserId = _defaultUserId });

            var update = new TaskItem { Title = "", UserId = _defaultUserId };

            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(created.Id, update));
        }

        [Fact]
        public async Task DeleteAsync_ExistingTask_RemovesTask()
        {
            var repository = GetRepository();
            var service = new TaskService(repository);

            var created = await service.CreateAsync(new TaskItem { Title = "ToDelete", UserId = _defaultUserId });
            await service.DeleteAsync(created.Id);

            var found = await service.GetByIdAsync(created.Id);
            found.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_TaskNotFound_DoesNotThrow()
        {
            var repository = GetRepository();
            var service = new TaskService(repository);

            await service.DeleteAsync(Guid.NewGuid());
            // No exception should be thrown
        }

        [Fact]
        public async Task SetStatusAsync_ExistingTask_SetsStatus()
        {
            var repository = GetRepository();
            var service = new TaskService(repository);

            var created = await service.CreateAsync(new TaskItem { Title = "StatusTask", UserId = _defaultUserId });

            var updated = await service.SetStatusAsync(created.Id, TaskItemStatus.Done);

            updated.Status.Should().Be(TaskItemStatus.Done);

            var found = await service.GetByIdAsync(created.Id);
            found.Should().NotBeNull();
            found!.Status.Should().Be(TaskItemStatus.Done);
        }

        [Fact]
        public async Task SetStatusAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            var repository = GetRepository();
            var service = new TaskService(repository);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.SetStatusAsync(Guid.NewGuid(), TaskItemStatus.Done));
        }

        private EfTaskRepository GetRepository()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<TaskDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new TaskDbContext(options);
            context.Database.EnsureCreated();
            
            // Create default test user to avoid FK constraints
            var user = new User
            {
                Id = _defaultUserId,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            context.SaveChanges();
            
            return new EfTaskRepository(context);
        }
    }
}
