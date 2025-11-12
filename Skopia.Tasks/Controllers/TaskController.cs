using Microsoft.AspNetCore.Mvc;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Interfaces;
using Skopia.Tasks.Application.Exceptions;

namespace Skopia.Tasks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        private readonly IProjectService _projectService;

        public TaskController(ITaskService taskService, IProjectService projectService)
        { 
            _taskService = taskService;
            _projectService = projectService;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            try
            {
                var tasks = await _taskService.GetByProjectIdAsync(projectId);
                return Ok(tasks);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{projectId}")]
        public async Task<IActionResult> Create(int projectId, [FromBody] TaskCreateDto dto)
        {
            try
            {
                var task = await _taskService.CreateAsync(projectId, dto);
                return CreatedAtAction(nameof(GetByProject), new { projectId = projectId }, task);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("{projectId}/{taskId}")]
        public async Task<IActionResult> UpdateAsync(int projectId, int taskId, [FromBody] TaskUpdateDto dto)
        {
            try
            {
                var updated = await _taskService.UpdateAsync(projectId, taskId, dto);
                return Ok(updated);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _taskService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
