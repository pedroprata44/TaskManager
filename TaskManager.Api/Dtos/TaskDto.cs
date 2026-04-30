using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Dtos;

public sealed class TaskDto
{
    [Required]
    public Guid? UserId { get; set; }

    [Required]
    [MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; } = false;
}
