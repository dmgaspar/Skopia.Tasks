using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Domain.Entities;
using Skopia.Tasks.Infrastructure.Persistence;

namespace Skopia.Tasks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Tasks)
                .ToListAsync();

            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound("Project not found");

            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProjectCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var result = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description
            };

            return CreatedAtAction(nameof(GetByIdAsync), new { id = project.Id }, result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProjectUpdateDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound("Project not found");

            project.Name = dto.Name;
            project.Description = dto.Description;

            await _context.SaveChangesAsync();

            var result = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description
            };

            return Ok(result);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound("Project not found");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
