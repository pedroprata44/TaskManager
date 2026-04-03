using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManager.Api.Models
{
    public class CreateTaskRequest
    {
        [Required]
        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("status")]
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("priority")]
        public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
    }
}
