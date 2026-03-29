using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Api.Models;

namespace TaskManager.Api.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<TaskItem> AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(Guid id);
        
        // New methods for ownership filtering
        Task<IEnumerable<TaskItem>> GetAllAsync(Guid userId);
        Task<TaskItem?> GetByIdAsync(Guid id, Guid userId);
    }
}
