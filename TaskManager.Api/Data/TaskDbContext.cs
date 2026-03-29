using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;

namespace TaskManager.Api.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks { get; set; }
}