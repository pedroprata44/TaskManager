using TaskManager.Api.Models;
using TaskModel = TaskManager.Api.Models.TaskModel;

namespace TaskManager.Api.Services;

public sealed class TaskService : ITaskService
{
    private readonly List<TaskModel> _tasks = new();

    public void Create(TaskModel task)
    {
        if (task.Id == Guid.Empty)
        {
            task.Id = Guid.NewGuid();
        }

        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _tasks.Add(task);
    }

    public IEnumerable<TaskModel> GetAll() => _tasks.ToList();

    public TaskModel GetById(Guid id)
    {
        var task = _tasks.FirstOrDefault(task => task.Id == id);
        if (task is null)
        {
            throw new KeyNotFoundException($"Task with id '{id}' was not found.");
        }

        return task;
    }

    public void Update(TaskModel task)
    {
        var index = _tasks.FindIndex(existing => existing.Id == task.Id);
        if (index < 0)
        {
            throw new KeyNotFoundException($"Task with id '{task.Id}' was not found.");
        }

        task.UpdatedAt = DateTime.UtcNow;
        _tasks[index] = task;
    }

    public void Delete(Guid id)
    {
        var task = GetById(id);
        _tasks.Remove(task);
    }
}
