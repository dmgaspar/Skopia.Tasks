using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Skopia.Tasks.Application.DTOs;

namespace Skopia.Tasks.Application.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllAsync();
        Task<ProjectDto> CreateAsync(ProjectDto dto);
    }
}
