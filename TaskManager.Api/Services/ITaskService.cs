using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<TaskItem> CreateAsync(TaskItem task);
        Task<TaskItem> UpdateAsync(Guid id, TaskItem task);
        Task DeleteAsync(Guid id);
        Task<TaskItem> SetStatusAsync(Guid id, TaskItemStatus status);
    }
}
