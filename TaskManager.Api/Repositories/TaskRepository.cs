using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Interfaces.Repositories;
using TaskManager.Api.Models;
using TaskModel = TaskManager.Api.Models.TaskModel;

namespace TaskManager.Api.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;

    public TaskRepository(TaskDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Create(TaskModel task)
    {
        if (task.Id == Guid.Empty)
        {
            task.Id = Guid.NewGuid();
        }

        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Add(task);
        _context.SaveChanges();
    }

    public IEnumerable<TaskModel> GetAll()
    {
        return _context.Tasks.AsNoTracking().ToList();
    }

    public TaskModel GetById(Guid id)
    {
        var task = _context.Tasks.Find(id);
        if (task is null)
        {
            throw new KeyNotFoundException($"Task with id '{id}' was not found.");
        }

        return task;
    }

    public void Update(TaskModel task)
    {
        var exists = _context.Tasks.Any(existing => existing.Id == task.Id);
        if (!exists)
        {
            throw new KeyNotFoundException($"Task with id '{task.Id}' was not found.");
        }

        task.UpdatedAt = DateTime.UtcNow;
        _context.Tasks.Update(task);
        _context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var task = GetById(id);
        _context.Tasks.Remove(task);
        _context.SaveChanges();
    }
}
