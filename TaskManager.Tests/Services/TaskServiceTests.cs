using TaskManager.Api.Services;
using TaskModel = TaskManager.Api.Models.TaskModel;

namespace TaskManager.Tests.Services;

public class TaskServiceTests
{
    [Fact]
    public void CreateTask_ShouldAddTask()
    {
        ITaskService service = new TaskService();
        var task = CreateRandomTask("Create");

        service.Create(task);

        var created = service.GetById(task.Id);

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.NotNull(created);
        Assert.Equal(task.Title, created.Title);
        Assert.Equal(task.Description, created.Description);
    }

    [Fact]
    public void GetAllTasks_ShouldReturnAllCreatedTasks()
    {
        ITaskService service = new TaskService();
        var first = CreateRandomTask("First");
        var second = CreateRandomTask("Second");

        service.Create(first);
        service.Create(second);

        var allTasks = service.GetAll();

        Assert.Contains(allTasks, t => t.Id == first.Id);
        Assert.Contains(allTasks, t => t.Id == second.Id);
        Assert.Equal(2, allTasks.Count());
    }

    [Fact]
    public void UpdateTask_ShouldPersistChanges()
    {
        ITaskService service = new TaskService();
        var task = CreateRandomTask("Original");

        service.Create(task);

        var updatedDescription = $"Updated-{Guid.NewGuid():N}";
        task.Description = updatedDescription;
        service.Update(task);

        var updated = service.GetById(task.Id);

        Assert.NotNull(updated);
        Assert.Equal(updatedDescription, updated.Description);
    }

    [Fact]
    public void DeleteTask_ShouldRemoveTask()
    {
        ITaskService service = new TaskService();
        var task = CreateRandomTask("Delete");

        service.Create(task);
        service.Delete(task.Id);

        var deleted = service.GetById(task.Id);

        Assert.Null(deleted);
    }

    [Fact]
    public void GetById_NonExistingId_ReturnsNull()
    {
        ITaskService service = new TaskService();

        var result = service.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public void UpdateTask_NonExistingTask_DoesNotCreateOrUpdate()
    {
        ITaskService service = new TaskService();
        var missingTask = new TaskModel
        {
            Id = Guid.NewGuid(),
            Title = $"Missing-{Guid.NewGuid():N}",
            Description = $"No such task {Guid.NewGuid():N}"
        };

        service.Update(missingTask);

        Assert.Null(service.GetById(missingTask.Id));
        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void DeleteTask_NonExistingId_DoesNotAffectExistingTasks()
    {
        ITaskService service = new TaskService();
        var task = CreateRandomTask("Present");

        service.Create(task);
        service.Delete(Guid.NewGuid());

        var allTasks = service.GetAll();

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
