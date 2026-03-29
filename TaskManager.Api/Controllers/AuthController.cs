using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest? request)
        {
            if (request == null)
                return BadRequest("Request cannot be null");

            try
            {
                var result = await _authService.RegisterAsync(request.Email, request.Username, request.Password);
                return CreatedAtAction(nameof(Login), result);
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid input data");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest? request)
        {
            if (request == null)
                return BadRequest("Request cannot be null");

            try
            {
                var result = await _authService.LoginAsync(request.Username, request.Password);
                return Ok(result);
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid input data");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials");
            }
            catch
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}