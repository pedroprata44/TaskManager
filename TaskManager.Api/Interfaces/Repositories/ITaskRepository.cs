using TaskManager.Api.Models;
using TaskModel = TaskManager.Api.Models.TaskModel;

namespace TaskManager.Api.Interfaces.Repositories;

public interface ITaskRepository
{
    void Create(TaskModel task);
    IEnumerable<TaskModel> GetAll();
    TaskModel GetById(Guid id);
    void Update(TaskModel task);
    void Delete(Guid id);
}
