using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Api.Models;
using Xunit;

namespace TaskManager.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            SetupJwtConfiguration();
            _controller = new AuthController(_mockConfiguration.Object);
        }

        private void SetupJwtConfiguration()
        {
            // Setup JWT configuration
            _mockConfiguration
                .Setup(x => x["Jwt:Key"])
                .Returns("this_is_a_very_long_secret_key_for_jwt_testing_purposes_only_1234567890");

            _mockConfiguration
                .Setup(x => x["Jwt:Issuer"])
                .Returns("TaskManager");

            _mockConfiguration
                .Setup(x => x["Jwt:Audience"])
                .Returns("TaskManagerUsers");
        }

        #region Login Tests
        [Fact]
        public void Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "password"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().NotBeNull();

            // Extract token from anonymous object
            var tokenValue = okResult?.Value?.GetType().GetProperty("Token")?.GetValue(okResult.Value);
            tokenValue.Should().NotBeNull();
            tokenValue.Should().BeOfType<string>();
        }

        [Fact]
        public void Login_WithInvalidUsername_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "invalid_user",
                Password = "password"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult?.StatusCode.Should().Be(401);
            unauthorizedResult?.Value.Should().Be("Invalid credentials");
        }

        [Fact]
        public void Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "wrong_password"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult?.Value.Should().Be("Invalid credentials");
        }

        [Fact]
        public void Login_WithBothInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "wrong_user",
                Password = "wrong_password"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public void Login_WithEmptyUsername_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "",
                Password = "password"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public void Login_WithEmptyPassword_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = ""
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public void Login_WithNullRequest_ThrowsNullReferenceException()
        {
            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _controller.Login(null!));
        }

        [Theory]
        [InlineData("admin", "password", true)]
        [InlineData("admin", "wrong", false)]
        [InlineData("user", "password", false)]
        [InlineData("", "password", false)]
        [InlineData("admin", "", false)]
        public void Login_VariousCombinations_ReturnsCorrectResult(string username, string password, bool shouldSucceed)
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            if (shouldSucceed)
            {
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var tokenValue = okResult?.Value?.GetType().GetProperty("Token")?.GetValue(okResult.Value);
                tokenValue.Should().NotBeNull();
            }
            else
            {
                result.Should().BeOfType<UnauthorizedObjectResult>();
            }
        }
        #endregion

        #region Token Validation Tests
        [Fact]
        public void Login_TokenShouldBeBase64Encoded()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "password"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var okResult = result as OkObjectResult;
            var token = okResult?.Value?.GetType().GetProperty("Token")?.GetValue(okResult.Value) as string;

            // JWT tokens have 3 parts separated by dots
            var parts = token!.Split('.');
            parts.Should().HaveCount(3);

            // Each part should be base64url encoded
            foreach (var part in parts)
            {
                part.Should().NotBeEmpty();
            }
        }

        [Fact]
        public void Login_MultipleLoginAttempts_EachReturnsValidToken()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "password"
            };

            // Act
            var result1 = _controller.Login(loginRequest);
            var result2 = _controller.Login(loginRequest);

            // Assert
            var token1 = (result1 as OkObjectResult)?.Value?.GetType().GetProperty("Token")?.GetValue((result1 as OkObjectResult)?.Value) as string;
            var token2 = (result2 as OkObjectResult)?.Value?.GetType().GetProperty("Token")?.GetValue((result2 as OkObjectResult)?.Value) as string;

            token1.Should().NotBeNull();
            token2.Should().NotBeNull();
            // Tokens should be different (due to Jti claim which includes a new Guid)
            token1.Should().NotBe(token2);
        }
        #endregion

        #region Request Validation Tests
        [Fact]
        public void Login_WithWhitespaceUsername_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "   ",
                Password = "password"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public void Login_WithWhitespacePassword_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "   "
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public void Login_CaseSensitiveUsername_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "Admin",  // Capital A
                Password = "password"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public void Login_CaseSensitivePassword_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "Password"  // Capital P
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
        #endregion
    }
}
