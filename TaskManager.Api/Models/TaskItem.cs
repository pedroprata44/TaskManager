using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Api.Models
{
    public enum TaskItemPriority
    {
        Low,
        Medium,
        High
    }

    public enum TaskItemStatus
    {
        Todo,
        InProgress,
        Done
    }

    public class TaskItem
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
