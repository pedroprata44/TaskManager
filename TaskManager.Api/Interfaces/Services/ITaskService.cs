using TaskManager.Api.Models;
using TaskModel = TaskManager.Api.Models.TaskModel;

namespace TaskManager.Api.Interfaces.Services;

public interface ITaskService
{
    void Create(TaskModel task);
    IEnumerable<TaskModel> GetAll();
    TaskModel GetById(Guid id);
    void Update(TaskModel task);
    void Delete(Guid id);
}
