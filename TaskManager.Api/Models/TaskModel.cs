namespace TaskManager.Api.Models;

public sealed class TaskModel
{
    public TaskModel()
    {
        CreatedAt = UpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public UserModel? User { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
