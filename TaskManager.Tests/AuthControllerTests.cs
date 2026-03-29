using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using Xunit;

namespace TaskManager.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        #region Register Tests
        [Fact]
        public async Task Register_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "newuser@test.com",
                Username = "newuser",
                Password = "SecurePass123"
            };
            var authResponse = new AuthResponse { Token = "valid.jwt.token" };
            _mockAuthService
                .Setup(x => x.RegisterAsync(registerRequest.Email, registerRequest.Username, registerRequest.Password))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult?.StatusCode.Should().Be(201);
            createdResult?.ActionName.Should().Be(nameof(AuthController.Login));
            
            var returnedResponse = createdResult?.Value as AuthResponse;
            returnedResponse?.Token.Should().Be("valid.jwt.token");

            _mockAuthService.Verify(
                x => x.RegisterAsync(registerRequest.Email, registerRequest.Username, registerRequest.Password),
                Times.Once);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "existing@test.com",
                Username = "newuser",
                Password = "SecurePass123"
            };
            _mockAuthService
                .Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Email already exists"));

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult?.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "new@test.com",
                Username = "existinguser",
                Password = "SecurePass123"
            };
            _mockAuthService
                .Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Username already exists"));

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_WithInvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "invalidemail",
                Username = "newuser",
                Password = "SecurePass123"
            };
            _mockAuthService
                .Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException("Invalid email format"));

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_WithWeakPassword_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "user@test.com",
                Username = "newuser",
                Password = "weak"
            };
            _mockAuthService
                .Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException("Password must be at least 8 characters"));

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Register(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region Login Tests
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "password"
            };
            var authResponse = new AuthResponse { Token = "valid.jwt.token" };
            _mockAuthService
                .Setup(x => x.LoginAsync(loginRequest.Username, loginRequest.Password))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.StatusCode.Should().Be(200);
            
            var returnedResponse = okResult?.Value as AuthResponse;
            returnedResponse?.Token.Should().Be("valid.jwt.token");

            _mockAuthService.Verify(
                x => x.LoginAsync(loginRequest.Username, loginRequest.Password),
                Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidUsername_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "invalid_user",
                Password = "password"
            };
            _mockAuthService
                .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult?.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "validuser",
                Password = "wrong_password"
            };
            _mockAuthService
                .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_WithEmptyUsername_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "",
                Password = "password"
            };
            _mockAuthService
                .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException());

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Login_WithEmptyPassword_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = ""
            };
            _mockAuthService
                .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException());

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Login_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Login(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData("testuser", "password", true)]
        [InlineData("testuser", "wrongpass", false)]
        [InlineData("wronguser", "password", false)]
        public async Task Login_VariousCombinations_ReturnsCorrectResult(string username, string password, bool shouldSucceed)
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };
            
            if (shouldSucceed)
            {
                _mockAuthService
                    .Setup(x => x.LoginAsync(username, password))
                    .ReturnsAsync(new AuthResponse { Token = "valid.token" });
            }
            else
            {
                _mockAuthService
                    .Setup(x => x.LoginAsync(username, password))
                    .ThrowsAsync(new UnauthorizedAccessException());
            }

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            if (shouldSucceed)
            {
                result.Should().BeOfType<OkObjectResult>();
            }
            else
            {
                result.Should().BeOfType<UnauthorizedObjectResult>();
            }
        }

        #endregion
    }
}
