using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Api.Models;
using TaskManager.Api.Repositories;

namespace TaskManager.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<TaskItem>> GetAllAsync() => _repository.GetAllAsync();

        public Task<TaskItem?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Title is required", nameof(task.Title));

            task.Status = TaskItemStatus.Todo;

            return await _repository.AddAsync(task);
        }

        public async Task<TaskItem> UpdateAsync(Guid id, TaskItem task)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                throw new KeyNotFoundException("Task not found");

            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Title is required", nameof(task.Title));

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.DueDate = task.DueDate;
            existing.Priority = task.Priority;
            existing.Status = task.Status;

            await _repository.UpdateAsync(existing);
            return existing;
        }

        public Task DeleteAsync(Guid id) => _repository.DeleteAsync(id);

        public async Task<TaskItem> SetStatusAsync(Guid id, TaskItemStatus status)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task is null)
                throw new KeyNotFoundException("Task not found");

            task.Status = status;
            await _repository.UpdateAsync(task);
            return task;
        }
    }
}
