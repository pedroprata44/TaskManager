using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using Xunit;

namespace TaskManager.Tests
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _controller = new TasksController(_mockTaskService.Object);
        }

        #region GetAll Tests
        [Fact]
        public async Task GetAll_ReturnsAllTasks()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 1", Status = TaskItemStatus.Todo },
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 2", Status = TaskItemStatus.InProgress }
            };
            _mockTaskService.Setup(s => s.GetAllAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().HaveCount(2);
            _mockTaskService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenNoTasks_ReturnsEmptyCollection()
        {
            // Arrange
            _mockTaskService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<TaskItem>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeEmpty();
        }
        #endregion

        #region GetById Tests
        [Fact]
        public async Task GetById_WhenTaskExists_ReturnsOkWithTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description",
                Status = TaskItemStatus.Todo,
                Priority = TaskItemPriority.High
            };
            _mockTaskService.Setup(s => s.GetByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            var result = await _controller.GetById(taskId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult?.Value.Should().Be(task);
            _mockTaskService.Verify(s => s.GetByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenTaskDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskService.Setup(s => s.GetByIdAsync(taskId)).ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _controller.GetById(taskId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
            _mockTaskService.Verify(s => s.GetByIdAsync(taskId), Times.Once);
        }
        #endregion

        #region Create Tests
        [Fact]
        public async Task Create_WithValidTask_ReturnsCreatedAtAction()
        {
            // Arrange
            var newTask = new TaskItem
            {
                Title = "New Task",
                Description = "New Description",
                Priority = TaskItemPriority.Medium
            };
            var createdTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = newTask.Title,
                Description = newTask.Description,
                Priority = newTask.Priority,
                Status = TaskItemStatus.Todo,
                CreatedAt = DateTime.UtcNow
            };
            _mockTaskService.Setup(s => s.CreateAsync(It.IsAny<TaskItem>())).ReturnsAsync(createdTask);

            // Act
            var result = await _controller.Create(newTask);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult?.ActionName.Should().Be(nameof(TasksController.GetById));
            createdResult?.RouteValues?["id"].Should().Be(createdTask.Id);
            createdResult?.Value.Should().Be(createdTask);
            _mockTaskService.Verify(s => s.CreateAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task Create_WithInvalidTitle_ThrowsArgumentException()
        {
            // Arrange
            var invalidTask = new TaskItem { Title = "" };
            _mockTaskService.Setup(s => s.CreateAsync(invalidTask))
                .ThrowsAsync(new ArgumentException("Title is required"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _controller.Create(invalidTask));
        }
        #endregion

        #region Update Tests
        [Fact]
        public async Task Update_WithExistingTask_ReturnsOkWithUpdatedTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateTask = new TaskItem
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Priority = TaskItemPriority.High
            };
            var updatedTask = new TaskItem
            {
                Id = taskId,
                Title = updateTask.Title,
                Description = updateTask.Description,
                Priority = updateTask.Priority,
                Status = TaskItemStatus.Todo
            };
            _mockTaskService.Setup(s => s.UpdateAsync(taskId, It.IsAny<TaskItem>())).ReturnsAsync(updatedTask);

            // Act
            var result = await _controller.Update(taskId, updateTask);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult?.Value.Should().Be(updatedTask);
            _mockTaskService.Verify(s => s.UpdateAsync(taskId, It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenTaskNotFound_ReturnsNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateTask = new TaskItem { Title = "Updated" };
            _mockTaskService.Setup(s => s.UpdateAsync(taskId, It.IsAny<TaskItem>()))
                .ThrowsAsync(new KeyNotFoundException("Task not found"));

            // Act
            var result = await _controller.Update(taskId, updateTask);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Update_WithInvalidTitle_ThrowsArgumentException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var invalidTask = new TaskItem { Title = "" };
            _mockTaskService.Setup(s => s.UpdateAsync(taskId, invalidTask))
                .ThrowsAsync(new ArgumentException("Title is required"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _controller.Update(taskId, invalidTask));
        }
        #endregion

        #region Delete Tests
        [Fact]
        public async Task Delete_WithExistingTask_ReturnsNoContent()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskService.Setup(s => s.DeleteAsync(taskId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(taskId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockTaskService.Verify(s => s.DeleteAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task Delete_SucceedsEvenIfTaskDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskService.Setup(s => s.DeleteAsync(taskId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(taskId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
        #endregion

        #region SetStatus Tests
        [Fact]
        public async Task SetStatus_WithExistingTask_ReturnsOkWithUpdatedStatus()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var newStatus = TaskItemStatus.InProgress;
            var updatedTask = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Status = newStatus,
                Priority = TaskItemPriority.Medium
            };
            _mockTaskService.Setup(s => s.SetStatusAsync(taskId, newStatus)).ReturnsAsync(updatedTask);

            // Act
            var result = await _controller.SetStatus(taskId, newStatus);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult?.Value.Should().Be(updatedTask);
            _mockTaskService.Verify(s => s.SetStatusAsync(taskId, newStatus), Times.Once);
        }

        [Fact]
        public async Task SetStatus_ChangeMultipleStatuses_UpdatesCorrectly()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var statuses = new[] { TaskItemStatus.InProgress, TaskItemStatus.Done };
            var tasks = new List<TaskItem>();

            foreach (var status in statuses)
            {
                _mockTaskService.Setup(s => s.SetStatusAsync(taskId, status))
                    .ReturnsAsync(new TaskItem { Id = taskId, Status = status, Title = "Task" });
            }

            // Act & Assert
            foreach (var status in statuses)
            {
                var result = await _controller.SetStatus(taskId, status);
                result.Result.Should().BeOfType<OkObjectResult>();
                var okResult = result.Result as OkObjectResult;
                (okResult?.Value as TaskItem)?.Status.Should().Be(status);
            }
        }

        [Fact]
        public async Task SetStatus_WhenTaskNotFound_ReturnsNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var newStatus = TaskItemStatus.Done;
            _mockTaskService.Setup(s => s.SetStatusAsync(taskId, newStatus))
                .ThrowsAsync(new KeyNotFoundException("Task not found"));

            // Act
            var result = await _controller.SetStatus(taskId, newStatus);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [InlineData(TaskItemStatus.Todo)]
        [InlineData(TaskItemStatus.InProgress)]
        [InlineData(TaskItemStatus.Done)]
        public async Task SetStatus_WithAllStatusValues_ReturnsOk(TaskItemStatus status)
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updatedTask = new TaskItem { Id = taskId, Title = "Task", Status = status };
            _mockTaskService.Setup(s => s.SetStatusAsync(taskId, status)).ReturnsAsync(updatedTask);

            // Act
            var result = await _controller.SetStatus(taskId, status);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }
        #endregion
    }
}
