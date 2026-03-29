using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Api.Models;

namespace TaskManager.Api.Repositories
{
    public class InMemoryTaskRepository : ITaskRepository
    {
        private readonly ConcurrentDictionary<Guid, TaskItem> _store = new();

        public Task<IEnumerable<TaskItem>> GetAllAsync() => Task.FromResult(_store.Values.AsEnumerable());

        public Task<TaskItem?> GetByIdAsync(Guid id)
        {
            _store.TryGetValue(id, out var task);
            return Task.FromResult(task);
        }

        public Task<TaskItem> AddAsync(TaskItem task)
        {
            task.Id = Guid.NewGuid();
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = task.CreatedAt;
            _store[task.Id] = task;
            return Task.FromResult(task);
        }

        public Task UpdateAsync(TaskItem task)
        {
            if (!_store.ContainsKey(task.Id))
                throw new KeyNotFoundException("Task not found");

            task.UpdatedAt = DateTime.UtcNow;
            _store[task.Id] = task;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _store.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<TaskItem>> GetAllAsync(Guid userId)
        {
            return Task.FromResult(_store.Values.Where(t => t.UserId == userId).AsEnumerable());
        }

        public Task<TaskItem?> GetByIdAsync(Guid id, Guid userId)
        {
            _store.TryGetValue(id, out var task);
            if (task != null && task.UserId != userId)
                return Task.FromResult<TaskItem?>(null);
            return Task.FromResult(task);
        }
    }
}
