using System.Collections.Generic;
using TaskManager.Api.Security;

namespace TaskManager.Api.Models;

public sealed class UserModel
{
    public UserModel()
    {
        CreatedAt = UpdatedAt = DateTime.UtcNow;
        IsActive = true;
        Tasks = new List<TaskModel>();
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<TaskModel> Tasks { get; set; }

    public void SetPassword(string password)
    {
        PasswordHash = PasswordHasher.HashPassword(password);
    }

    public bool VerifyPassword(string password)
    {
        return PasswordHasher.VerifyPassword(password, PasswordHash);
    }
}
