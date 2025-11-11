using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Interfaces;
using Skopia.Tasks.Application.Exceptions;
using Skopia.Tasks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Domain.Entities;

namespace Skopia.Tasks.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            return await _context.Projects
                .Select(p => new ProjectDto { Id = p.Id, Name = p.Name })
                .ToListAsync();
        }

        public async Task<ProjectDto> CreateAsync(ProjectDto dto)
        {
            var project = new Project { Name = dto.Name };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            dto.Id = project.Id;
            return dto;
        }
    }
}
