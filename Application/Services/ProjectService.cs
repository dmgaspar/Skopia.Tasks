using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Interfaces;
using Skopia.Tasks.Domain.Entities;
using Skopia.Tasks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Skopia.Tasks.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            return await _context.Projects
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
        }

        // GET BY ID
        public async Task<ProjectDto?> GetByIdAsync(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return null;

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name
            };
        }

        // CREATE
        public async Task<ProjectDto> CreateAsync(ProjectDto dto)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            dto.Id = project.Id;
            return dto;
        }

        // UPDATE
        public async Task<ProjectDto?> UpdateAsync(int id, ProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return null;

            project.Name = dto.Name ?? project.Name;
            project.Description = dto.Description ?? project.Description;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name
            };
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return false;

            if (project.Tasks != null && project.Tasks.Any(t => t.Status != Domain.Enums.TaskStatus.Done))
                return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
