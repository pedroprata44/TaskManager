using TaskManager.Api.Interfaces.Repositories;
using TaskManager.Api.Repositories;
using TaskManager.Api.Models;
using TaskModel = TaskManager.Api.Models.TaskModel;

namespace TaskManager.Tests.Repositories;

public class TaskRepositoryTests
{
    [Fact]
    public void CreateTask_ShouldAddTask()
    {
        ITaskRepository repository = new TaskRepository();
        var task = CreateRandomTask("Create");

        repository.Create(task);

        var created = repository.GetById(task.Id);

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.NotNull(created);
        Assert.Equal(task.Title, created.Title);
        Assert.Equal(task.Description, created.Description);
    }

    [Fact]
    public void GetAllTasks_ShouldReturnAllCreatedTasks()
    {
        ITaskRepository repository = new TaskRepository();
        var first = CreateRandomTask("First");
        var second = CreateRandomTask("Second");

        repository.Create(first);
        repository.Create(second);

        var allTasks = repository.GetAll();

        Assert.Contains(allTasks, t => t.Id == first.Id);
        Assert.Contains(allTasks, t => t.Id == second.Id);
        Assert.Equal(2, allTasks.Count());
    }

    [Fact]
    public void UpdateTask_ShouldPersistChanges()
    {
        ITaskRepository repository = new TaskRepository();
        var task = CreateRandomTask("Original");

        repository.Create(task);

        var updatedDescription = $"Updated-{Guid.NewGuid():N}";
        task.Description = updatedDescription;
        repository.Update(task);

        var updated = repository.GetById(task.Id);

        Assert.Equal(updatedDescription, updated.Description);
    }

    [Fact]
    public void DeleteTask_ShouldRemoveTask()
    {
        ITaskRepository repository = new TaskRepository();
        var task = CreateRandomTask("Delete");

        repository.Create(task);
        repository.Delete(task.Id);

        Assert.Throws<KeyNotFoundException>(() => repository.GetById(task.Id));
    }

    [Fact]
    public void GetById_NonExistingId_ThrowsKeyNotFoundException()
    {
        ITaskRepository repository = new TaskRepository();

        Assert.Throws<KeyNotFoundException>(() => repository.GetById(Guid.NewGuid()));
    }

    [Fact]
    public void UpdateTask_NonExistingTask_ThrowsKeyNotFoundException()
    {
        ITaskRepository repository = new TaskRepository();
        var missingTask = new TaskModel
        {
            Id = Guid.NewGuid(),
            Title = $"Missing-{Guid.NewGuid():N}",
            Description = $"No such task {Guid.NewGuid():N}"
        };

        Assert.Throws<KeyNotFoundException>(() => repository.Update(missingTask));
        Assert.Empty(repository.GetAll());
    }

    [Fact]
    public void DeleteTask_NonExistingId_ThrowsKeyNotFoundException()
    {
        ITaskRepository repository = new TaskRepository();
        var task = CreateRandomTask("Present");

        repository.Create(task);

        Assert.Throws<KeyNotFoundException>(() => repository.Delete(Guid.NewGuid()));

        var allTasks = repository.GetAll();
        Assert.Single(allTasks);
        Assert.Equal(task.Id, allTasks.Single().Id);
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
