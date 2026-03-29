using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using Xunit;

namespace TaskManager.Tests
{
    public class AuthServiceTests
    {
        private TaskDbContext GetDbContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<TaskDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new TaskDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        #region Register Tests
        [Fact]
        public async Task RegisterAsync_WithValidData_CreatesUserAndReturnsToken()
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);
            var email = "newuser@test.com";
            var username = "newuser";
            var password = "SecurePass123";

            // Act
            var result = await service.RegisterAsync(email, username, password);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            
            var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            userInDb.Should().NotBeNull();
            userInDb!.Username.Should().Be(username);
            userInDb.Id.Should().NotBe(Guid.Empty);
            userInDb.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task RegisterAsync_WithDuplicateEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);
            var email = "duplicate@test.com";
            var username1 = "user1";
            var username2 = "user2";
            var password = "SecurePass123";

            await service.RegisterAsync(email, username1, password);

            // Act
            Func<Task> act = () => service.RegisterAsync(email, username2, password);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Email already exists*");
        }

        [Fact]
        public async Task RegisterAsync_WithDuplicateUsername_ThrowsInvalidOperationException()
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);
            var email1 = "user1@test.com";
            var email2 = "user2@test.com";
            var username = "duplicateuser";
            var password = "SecurePass123";

            await service.RegisterAsync(email1, username, password);

            // Act
            Func<Task> act = () => service.RegisterAsync(email2, username, password);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Username already exists*");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RegisterAsync_WithInvalidEmail_ThrowsArgumentException(string email)
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                service.RegisterAsync(email, "validuser", "ValidPass123"));
        }

        [Theory]
        [InlineData("notanemail")]
        [InlineData("user@")]
        [InlineData("@domain.com")]
        [InlineData("user @domain.com")]
        public async Task RegisterAsync_WithInvalidEmailFormat_ThrowsArgumentException(string email)
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                service.RegisterAsync(email, "validuser", "ValidPass123"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RegisterAsync_WithInvalidUsername_ThrowsArgumentException(string username)
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                service.RegisterAsync("user@test.com", username, "ValidPass123"));
        }

        [Theory]
        [InlineData("ab")]  // Less than 3 chars
        [InlineData("a")]
        public async Task RegisterAsync_WithUsernameTooShort_ThrowsArgumentException(string username)
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                service.RegisterAsync("user@test.com", username, "ValidPass123"));
            
            exception.Message.Should().Contain("Username must be at least 3 characters");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RegisterAsync_WithInvalidPassword_ThrowsArgumentException(string password)
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                service.RegisterAsync("user@test.com", "validuser", password));
        }

        [Theory]
        [InlineData("short")]     // Less than 8 chars
        [InlineData("1234567")]   // 7 chars
        [InlineData("Pass@1")]    // 6 chars
        public async Task RegisterAsync_WithPasswordTooShort_ThrowsArgumentException(string password)
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                service.RegisterAsync("user@test.com", "validuser", password));
            
            exception.Message.Should().Contain("Password must be at least 8 characters");
        }

        [Fact]
        public async Task RegisterAsync_PasswordIsHashed_NotStoredAsPlaintext()
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);
            var password = "MySecurePassword123";
            var email = "user@test.com";

            // Act
            await service.RegisterAsync(email, "testuser", password);

            // Assert
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            user!.PasswordHash.Should().NotBe(password);
            user.PasswordHash.Should().StartWith("$2"); // bcrypt hash starts with $2
        }

        #endregion

        #region Login Tests
        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);
            var email = "login@test.com";
            var username = "loginuser";
            var password = "ValidPassword123";

            await service.RegisterAsync(email, username, password);

            // Act
            var result = await service.LoginAsync(username, password);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoginAsync_WithIncorrectPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);
            var email = "test@test.com";
            var username = "testuser";
            var correctPassword = "CorrectPass123";
            var wrongPassword = "WrongPass123";

            await service.RegisterAsync(email, username, correctPassword);

            // Act
            Func<Task> act = () => service.LoginAsync(username, wrongPassword);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Invalid credentials*");
        }

        [Fact]
        public async Task LoginAsync_WithNonexistentUsername_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);

            // Act
            Func<Task> act = () => service.LoginAsync("nonexistent", "anypassword");

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Invalid credentials*");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task LoginAsync_WithEmptyUsername_ThrowsArgumentException(string username)
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                service.LoginAsync(username, "password"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task LoginAsync_WithEmptyPassword_ThrowsArgumentException(string password)
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                service.LoginAsync("username", password));
        }

        [Fact]
        public async Task LoginAsync_ReturnedTokenIsValid()
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);
            var email = "validtoken@test.com";
            var username = "tokenuser";
            var password = "ValidPassword123";

            await service.RegisterAsync(email, username, password);

            // Act
            var result = await service.LoginAsync(username, password);

            // Assert
            result.Token.Should().NotBeNullOrEmpty();
            // Token should be a valid JWT (contains dots)
            result.Token.Split('.').Length.Should().Be(3);
        }

        #endregion

        #region Multiple Registration/Login Flow
        [Fact]
        public async Task MultipleUsers_EachCanLoginWithOwnCredentials()
        {
            // Arrange
            var context = GetDbContext();
            var service = new AuthService(context);
            var user1Email = "user1@test.com";
            var user1Username = "user1";
            var user1Password = "Password1@@@";
            
            var user2Email = "user2@test.com";
            var user2Username = "user2";
            var user2Password = "Password2@@@";

            // Act - Register and login both users
            await service.RegisterAsync(user1Email, user1Username, user1Password);
            var token1 = (await service.LoginAsync(user1Username, user1Password)).Token;

            await service.RegisterAsync(user2Email, user2Username, user2Password);
            var token2 = (await service.LoginAsync(user2Username, user2Password)).Token;

            // Assert
            token1.Should().NotBe(token2);
            token1.Should().NotBeNullOrEmpty();
            token2.Should().NotBeNullOrEmpty();
        }

        #endregion
    }
}
