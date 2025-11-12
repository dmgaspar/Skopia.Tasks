using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Skopia.Tasks.Application.DTOs;

namespace Skopia.Tasks.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetByProjectIdAsync(int projectId);
        Task<TaskDto> CreateAsync(int projectId, TaskCreateDto dto);
        Task<TaskDto> UpdateAsync(int projectId, int taskId, TaskUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
