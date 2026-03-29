using System;
using System.Collections.Generic;
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

        [HttpGet]
        public async Task<IEnumerable<TaskItem>> GetAll() => await _taskService.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetById(Guid id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task is null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TaskItem>> Create([FromBody] TaskItem task)
        {
            var created = await _taskService.CreateAsync(task);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<TaskItem>> Update(Guid id, [FromBody] TaskItem task)
        {
            try
            {
                var updated = await _taskService.UpdateAsync(id, task);
                return Ok(updated);
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
            await _taskService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [Authorize]
        public async Task<ActionResult<TaskItem>> SetStatus(Guid id, [FromBody] TaskItemStatus status)
        {
            try
            {
                var updated = await _taskService.SetStatusAsync(id, status);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}