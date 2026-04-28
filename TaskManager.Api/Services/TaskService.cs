using TaskManager.Api.Interfaces.Repositories;
using TaskManager.Api.Interfaces.Services;
using TaskManager.Api.Models;
using TaskManager.Api.Repositories;
using TaskModel = TaskManager.Api.Models.TaskModel;

namespace TaskManager.Api.Services;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public void Create(TaskModel task)
    {
        _repository.Create(task);
    }

    public IEnumerable<TaskModel> GetAll() => _repository.GetAll();

    public TaskModel GetById(Guid id) => _repository.GetById(id);

    public void Update(TaskModel task) => _repository.Update(task);

    public void Delete(Guid id) => _repository.Delete(id);
}
