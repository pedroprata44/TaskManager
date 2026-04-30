using TaskManager.Api.Security;

namespace TaskManager.Tests.Security;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_ReturnsDifferentStringThanPlainPassword()
    {
        var password = "StrongPassword!123";

        var hash = PasswordHasher.HashPassword(password);

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.NotEqual(password, hash);
        Assert.True(hash.StartsWith("$2"));
    }

    [Fact]
    public void VerifyPassword_ReturnsTrueForValidPassword()
    {
        var password = "StrongPassword!123";
        var hash = PasswordHasher.HashPassword(password);

        Assert.True(PasswordHasher.VerifyPassword(password, hash));
    }

    [Fact]
    public void VerifyPassword_ReturnsFalseForInvalidPassword()
    {
        var password = "StrongPassword!123";
        var hash = PasswordHasher.HashPassword(password);

        Assert.False(PasswordHasher.VerifyPassword("WrongPassword", hash));
    }

    [Fact]
    public void HashPassword_ThrowsOnEmptyPassword()
    {
        Assert.Throws<ArgumentException>(() => PasswordHasher.HashPassword(string.Empty));
    }

    [Fact]
    public void VerifyPassword_ReturnsFalseForEmptyPasswordOrHash()
    {
        var hash = PasswordHasher.HashPassword("StrongPassword!123");

        Assert.False(PasswordHasher.VerifyPassword(string.Empty, hash));
        Assert.False(PasswordHasher.VerifyPassword("StrongPassword!123", string.Empty));
    }
}
