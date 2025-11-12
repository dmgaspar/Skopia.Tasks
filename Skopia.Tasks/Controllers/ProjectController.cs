using Microsoft.AspNetCore.Mvc;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Interfaces;

namespace Skopia.Tasks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var projects = await _projectService.GetAllAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
                return NotFound($"Projeto com ID {id} não encontrado.");

            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProjectCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdProject = await _projectService.CreateAsync(dto);
            return Created($"api/Project/{createdProject.Id}", createdProject);

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProjectUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _projectService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound($"Projeto com ID {id} não encontrado.");

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _projectService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result.Message);

            return NoContent();
        }

    }
}
