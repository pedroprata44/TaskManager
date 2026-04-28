using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Interfaces.Services;
using TaskManager.Api.Repositories;
using TaskManager.Api.Services;
using TaskModel = TaskManager.Api.Models.TaskModel;

namespace TaskManager.Tests.Services;

public class TaskServiceTests : IDisposable
{
    private readonly TaskDbContext _context;
    private readonly ITaskService _service;

    public TaskServiceTests()
    {
        _context = CreateContext();
        _service = new TaskService(new TaskRepository(_context));
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public void CreateTask_ShouldAddTask()
    {
        var task = CreateRandomTask("Create");

        _service.Create(task);

        var created = _service.GetById(task.Id);

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.NotNull(created);
        Assert.Equal(task.Title, created.Title);
        Assert.Equal(task.Description, created.Description);
    }

    [Fact]
    public void GetAllTasks_ShouldReturnAllCreatedTasks()
    {
        var first = CreateRandomTask("First");
        var second = CreateRandomTask("Second");

        _service.Create(first);
        _service.Create(second);

        var allTasks = _service.GetAll();

        Assert.Contains(allTasks, t => t.Id == first.Id);
        Assert.Contains(allTasks, t => t.Id == second.Id);
        Assert.Equal(2, allTasks.Count());
    }

    [Fact]
    public void UpdateTask_ShouldPersistChanges()
    {
        var task = CreateRandomTask("Original");

        _service.Create(task);

        var updatedDescription = $"Updated-{Guid.NewGuid():N}";
        task.Description = updatedDescription;
        _service.Update(task);

        var updated = _service.GetById(task.Id);

        Assert.NotNull(updated);
        Assert.Equal(updatedDescription, updated.Description);
    }

    [Fact]
    public void DeleteTask_ShouldRemoveTask()
    {
        var task = CreateRandomTask("Delete");

        _service.Create(task);
        _service.Delete(task.Id);

        Assert.Throws<KeyNotFoundException>(() => _service.GetById(task.Id));
    }

    [Fact]
    public void GetById_NonExistingId_ThrowsKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => _service.GetById(Guid.NewGuid()));
    }

    [Fact]
    public void UpdateTask_NonExistingTask_ThrowsKeyNotFoundException()
    {
        var missingTask = new TaskModel
        {
            Id = Guid.NewGuid(),
            Title = $"Missing-{Guid.NewGuid():N}",
            Description = $"No such task {Guid.NewGuid():N}"
        };

        Assert.Throws<KeyNotFoundException>(() => _service.Update(missingTask));
        Assert.Empty(_service.GetAll());
    }

    [Fact]
    public void DeleteTask_NonExistingId_ThrowsKeyNotFoundException()
    {
        var task = CreateRandomTask("Present");

        _service.Create(task);

        Assert.Throws<KeyNotFoundException>(() => _service.Delete(Guid.NewGuid()));

        var allTasks = _service.GetAll();

        Assert.Single(allTasks);
        Assert.Equal(task.Id, allTasks.Single().Id);
    }

    private static TaskDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TaskDbContext(options);
    }

    private static TaskModel CreateRandomTask(string prefix)
    {
        return new TaskModel
        {
            Title = $"{prefix}-{Guid.NewGuid():N}",
            Description = $"Description-{Guid.NewGuid():N}"
        };
    }
}
