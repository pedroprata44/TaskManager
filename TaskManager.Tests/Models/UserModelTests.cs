using TaskModel = TaskManager.Api.Models.UserModel;

namespace TaskManager.Tests.Models;

public class UserModelTests
{
    [Fact]
    public void User_DefaultValues_AreInitializedCorrectly()
    {
        var user = new TaskModel();

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(string.Empty, user.Username);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.True(user.IsActive);
        Assert.NotEqual(default, user.CreatedAt);
        Assert.NotEqual(default, user.UpdatedAt);
        Assert.Equal(user.CreatedAt, user.UpdatedAt);
    }

    [Fact]
    public void SetPassword_HashesPasswordWithBcryptAndVerifiesCorrectly()
    {
        var user = new TaskModel();
        user.SetPassword("SuperSecret123!");

        Assert.NotEqual(string.Empty, user.PasswordHash);
        Assert.NotEqual("SuperSecret123!", user.PasswordHash);
        Assert.True(user.VerifyPassword("SuperSecret123!"));
        Assert.False(user.VerifyPassword("WrongPassword"));
    }
}
