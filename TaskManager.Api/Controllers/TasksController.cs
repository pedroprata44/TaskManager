using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Extrai o ID do usuário do JWT token através do claim NameIdentifier
        /// </summary>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new InvalidOperationException("Unable to extract user ID from JWT token");
            }
            return userId;
        }

        [HttpGet]
        public async Task<IEnumerable<TaskItem>> GetAll()
        {
            var userId = GetCurrentUserId();
            return await _taskService.GetAllAsync(userId);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetById(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var task = await _taskService.GetByIdAsync(id, userId);
                return Ok(task);
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TaskItem>> Create([FromBody] TaskItem task)
        {
            var userId = GetCurrentUserId();
            task.UserId = userId;
            var created = await _taskService.CreateAsync(task);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<TaskItem>> Update(Guid id, [FromBody] TaskItem task)
        {
            try
            {
                var userId = GetCurrentUserId();
                var updated = await _taskService.UpdateAsync(id, task, userId);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _taskService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize]
        public async Task<ActionResult<TaskItem>> SetStatus(Guid id, [FromBody] TaskItemStatus status)
        {
            try
            {
                var userId = GetCurrentUserId();
                var updated = await _taskService.SetStatusAsync(id, status, userId);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}