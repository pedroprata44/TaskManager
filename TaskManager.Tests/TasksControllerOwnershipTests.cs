using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using Xunit;

namespace TaskManager.Tests
{
    public class TasksControllerOwnershipTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly TasksController _controller;
        private readonly Guid _testUserId = Guid.NewGuid();

        public TasksControllerOwnershipTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _controller = new TasksController(_mockTaskService.Object);
            SetupAuthenticatedUser();
        }

        private void SetupAuthenticatedUser()
        {
            // Setup controller with authenticated user via User principal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString()),
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            
            var httpContext = new DefaultHttpContext
            {
                User = principal
            };
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        #region GetAll with UserId Tests
        [Fact]
        public async Task GetAll_ExtractsUserIdFromJwt_PassesToService()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 1", UserId = _testUserId },
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 2", UserId = _testUserId }
            };
            _mockTaskService
                .Setup(s => s.GetAllAsync(_testUserId))
                .ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(t => t.UserId.Should().Be(_testUserId));
            _mockTaskService.Verify(s => s.GetAllAsync(_testUserId), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenNoTasksForUser_ReturnsEmptyList()
        {
            // Arrange
            _mockTaskService
                .Setup(s => s.GetAllAsync(_testUserId))
                .ReturnsAsync(new List<TaskItem>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeEmpty();
            _mockTaskService.Verify(s => s.GetAllAsync(_testUserId), Times.Once);
        }

        #endregion

        #region GetById with Ownership Tests
        [Fact]
        public async Task GetById_WithCorrectOwner_ReturnsOkWithTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem
            {
                Id = taskId,
                Title = "My Task",
                UserId = _testUserId
            };
            _mockTaskService
                .Setup(s => s.GetByIdAsync(taskId, _testUserId))
                .ReturnsAsync(task);

            // Act
            var result = await _controller.GetById(taskId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult?.Value.Should().Be(task);
            _mockTaskService.Verify(s => s.GetByIdAsync(taskId, _testUserId), Times.Once);
        }

        [Fact]
        public async Task GetById_WithWrongOwner_ThrowsUnauthorizedAndNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskService
                .Setup(s => s.GetByIdAsync(taskId, _testUserId))
                .ThrowsAsync(new UnauthorizedAccessException("Not authorized"));

            // Act
            var result = await _controller.GetById(taskId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region Create with UserId Tests
        [Fact]
        public async Task Create_SetsUserIdFromJwt_ReturnsCreatedAtAction()
        {
            // Arrange
            var newTask = new CreateTaskRequest
            {
                Title = "New Task",
                Description = "New Description"
            };
            var createdTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = newTask.Title,
                Description = newTask.Description,
                UserId = _testUserId,  // Should be set from JWT
                Status = TaskItemStatus.Todo,
                CreatedAt = DateTime.UtcNow
            };
            _mockTaskService
                .Setup(s => s.CreateAsync(It.Is<TaskItem>(t => 
                    t.Title == newTask.Title && t.UserId == _testUserId)))
                .ReturnsAsync(createdTask);

            // Act
            var result = await _controller.Create(newTask);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedTask = createdResult?.Value as TaskItem;
            returnedTask?.UserId.Should().Be(_testUserId);
        }

        [Fact]
        public async Task Create_EnforcesUserIdFromJwt_IgnoresClientProvidedUserId()
        {
            // Arrange
            var clientProvidedUserId = Guid.NewGuid(); // Different user ID from client
            var newTask = new CreateTaskRequest
            {
                Title = "Task"
                // Client can't set UserId in CreateTaskRequest, it's from JWT
            };
            var createdTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = newTask.Title,
                UserId = _testUserId,  // Should override with JWT userId
                Status = TaskItemStatus.Todo,
                CreatedAt = DateTime.UtcNow
            };
            _mockTaskService
                .Setup(s => s.CreateAsync(It.Is<TaskItem>(t => t.UserId == _testUserId)))
                .ReturnsAsync(createdTask);

            // Act
            var result = await _controller.Create(newTask);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedTask = createdResult?.Value as TaskItem;
            returnedTask?.UserId.Should().Be(_testUserId);
            returnedTask?.UserId.Should().NotBe(clientProvidedUserId);
        }

        #endregion

        #region Update with Ownership Tests
        [Fact]
        public async Task Update_WithCorrectOwner_UpdatesTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateTask = new TaskItem { Title = "Updated" };
            var updatedTask = new TaskItem
            {
                Id = taskId,
                Title = "Updated",
                UserId = _testUserId,
                Status = TaskItemStatus.Todo
            };
            _mockTaskService
                .Setup(s => s.UpdateAsync(taskId, It.IsAny<TaskItem>(), _testUserId))
                .ReturnsAsync(updatedTask);

            // Act
            var result = await _controller.Update(taskId, updateTask);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            _mockTaskService.Verify(s => s.UpdateAsync(taskId, It.IsAny<TaskItem>(), _testUserId), Times.Once);
        }

        [Fact]
        public async Task Update_WithWrongOwner_Returns403Forbidden()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateTask = new TaskItem { Title = "Updated" };
            _mockTaskService
                .Setup(s => s.UpdateAsync(taskId, It.IsAny<TaskItem>(), _testUserId))
                .ThrowsAsync(new UnauthorizedAccessException("Not authorized"));

            // Act
            var result = await _controller.Update(taskId, updateTask);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            var objResult = result.Result as ObjectResult;
            objResult?.StatusCode.Should().Be(403);
        }

        #endregion

        #region Delete with Ownership Tests
        [Fact]
        public async Task Delete_WithCorrectOwner_DeletesTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskService
                .Setup(s => s.DeleteAsync(taskId, _testUserId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(taskId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockTaskService.Verify(s => s.DeleteAsync(taskId, _testUserId), Times.Once);
        }

        [Fact]
        public async Task Delete_WithWrongOwner_Returns403Forbidden()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskService
                .Setup(s => s.DeleteAsync(taskId, _testUserId))
                .ThrowsAsync(new UnauthorizedAccessException("Not authorized"));

            // Act
            var result = await _controller.Delete(taskId);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objResult = result as ObjectResult;
            objResult?.StatusCode.Should().Be(403);
        }

        #endregion

        #region SetStatus with Ownership Tests
        [Fact]
        public async Task SetStatus_WithCorrectOwner_ChangesStatus()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var newStatus = TaskItemStatus.InProgress;
            var updatedTask = new TaskItem
            {
                Id = taskId,
                Title = "Task",
                Status = newStatus,
                UserId = _testUserId
            };
            _mockTaskService
                .Setup(s => s.SetStatusAsync(taskId, newStatus, _testUserId))
                .ReturnsAsync(updatedTask);

            // Act
            var result = await _controller.SetStatus(taskId, newStatus);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            _mockTaskService.Verify(s => s.SetStatusAsync(taskId, newStatus, _testUserId), Times.Once);
        }

        [Fact]
        public async Task SetStatus_WithWrongOwner_Returns403Forbidden()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var newStatus = TaskItemStatus.Done;
            _mockTaskService
                .Setup(s => s.SetStatusAsync(taskId, newStatus, _testUserId))
                .ThrowsAsync(new UnauthorizedAccessException("Not authorized"));

            // Act
            var result = await _controller.SetStatus(taskId, newStatus);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            var objResult = result.Result as ObjectResult;
            objResult?.StatusCode.Should().Be(403);
        }

        #endregion

        #region Error Handling Tests
        [Fact]
        public async Task Controller_WithoutAuthenticatedUser_MissingClaim_ThrowsException()
        {
            // Arrange - Create controller without user
            var controllerNoAuth = new TasksController(_mockTaskService.Object);
            controllerNoAuth.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act & Assert - Should handle missing claim gracefully
            await Assert.ThrowsAsync<InvalidOperationException>(() => controllerNoAuth.GetAll());
        }

        #endregion
    }
}
