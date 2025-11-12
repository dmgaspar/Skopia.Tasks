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
                    Name = p.Name,
                    Description = p.Description ?? string.Empty
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
                Name = project.Name,
                Description = project.Description ?? string.Empty
            };
        }

        // CREATE
        public async Task<ProjectDto> CreateAsync(ProjectCreateDto dto)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description
            };
        }

        // UPDATE
        public async Task<ProjectDto?> UpdateAsync(int id, ProjectUpdateDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return null;

            project.Name = dto.Name;
            project.Description = dto.Description;
            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description
            };
        }

        // DELETE
        public async Task<DeleteResultDto> DeleteAsync(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return new DeleteResultDto
                {
                    Success = false,
                    Message = $"Projeto com ID {id} não encontrado."
                };
            }

            if (project.Tasks != null && project.Tasks.Any(t => t.Status != Domain.Enums.TaskStatus.Concluida))
            {
                return new DeleteResultDto
                {
                    Success = false,
                    Message = "Não é possível excluir um projeto que ainda tenha tarefas pendentes."
                };
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return new DeleteResultDto
            {
                Success = true,
                Message = "Projeto excluído com sucesso."
            };
        }
    }
}
