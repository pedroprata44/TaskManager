using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services
{
    public interface ITaskService
    {
        // Legacy methods (without userId) - mantém compatibilidade
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<TaskItem> CreateAsync(TaskItem task);
        Task<TaskItem> UpdateAsync(Guid id, TaskItem task);
        Task DeleteAsync(Guid id);
        Task<TaskItem> SetStatusAsync(Guid id, TaskItemStatus status);
        
        // New methods with userId (ownership validation)
        Task<IEnumerable<TaskItem>> GetAllAsync(Guid userId);
        Task<TaskItem?> GetByIdAsync(Guid id, Guid userId);
        Task<TaskItem> UpdateAsync(Guid id, TaskItem task, Guid userId);
        Task DeleteAsync(Guid id, Guid userId);
        Task<TaskItem> SetStatusAsync(Guid id, TaskItemStatus status, Guid userId);
    }
}
