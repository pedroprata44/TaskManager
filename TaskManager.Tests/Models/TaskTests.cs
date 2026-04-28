using TaskModel = TaskManager.Api.Models.Task;

namespace TaskManager.Tests.Models;

public class TaskTests
{
    [Fact]
    public void Task_DefaultValues_AreInitializedCorrectly()
    {
        var task = new TaskModel();

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal(string.Empty, task.Title);
        Assert.Equal(string.Empty, task.Description);
        Assert.False(task.IsCompleted);
        Assert.NotEqual(default, task.CreatedAt);
        Assert.NotEqual(default, task.UpdatedAt);
        Assert.Equal(task.CreatedAt, task.UpdatedAt);
    }
}
