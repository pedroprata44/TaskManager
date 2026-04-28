using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Interfaces.Repositories;
using TaskManager.Api.Repositories;
using TaskManager.Api.Models;
using TaskModel = TaskManager.Api.Models.TaskModel;

namespace TaskManager.Tests.Repositories;

public class TaskRepositoryTests : IDisposable
{
    private readonly TaskDbContext _context;
    private readonly ITaskRepository _repository;

    public TaskRepositoryTests()
    {
        _context = CreateContext();
        _repository = new TaskRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public void CreateTask_ShouldAddTask()
    {
        var task = CreateRandomTask("Create");

        _repository.Create(task);

        var created = _repository.GetById(task.Id);

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

        _repository.Create(first);
        _repository.Create(second);

        var allTasks = _repository.GetAll();

        Assert.Contains(allTasks, t => t.Id == first.Id);
        Assert.Contains(allTasks, t => t.Id == second.Id);
        Assert.Equal(2, allTasks.Count());
    }

    [Fact]
    public void UpdateTask_ShouldPersistChanges()
    {
        var task = CreateRandomTask("Original");

        _repository.Create(task);

        var updatedDescription = $"Updated-{Guid.NewGuid():N}";
        task.Description = updatedDescription;
        _repository.Update(task);

        var updated = _repository.GetById(task.Id);

        Assert.Equal(updatedDescription, updated.Description);
    }

    [Fact]
    public void DeleteTask_ShouldRemoveTask()
    {
        var task = CreateRandomTask("Delete");

        _repository.Create(task);
        _repository.Delete(task.Id);

        Assert.Throws<KeyNotFoundException>(() => _repository.GetById(task.Id));
    }

    [Fact]
    public void GetById_NonExistingId_ThrowsKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => _repository.GetById(Guid.NewGuid()));
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

        Assert.Throws<KeyNotFoundException>(() => _repository.Update(missingTask));
        Assert.Empty(_repository.GetAll());
    }

    [Fact]
    public void DeleteTask_NonExistingId_ThrowsKeyNotFoundException()
    {
        var task = CreateRandomTask("Present");

        _repository.Create(task);

        Assert.Throws<KeyNotFoundException>(() => _repository.Delete(Guid.NewGuid()));

        var allTasks = _repository.GetAll();
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
