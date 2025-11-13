using Skopia.Tasks.Domain.Entities;

namespace Skopia.Tasks.Application.Interfaces
{
    public interface IHistoryService
    {
        Task<List<TaskHistory>> GetAllAsync();
        Task<List<TaskHistory>> GetByTaskAsync(int taskItemId);
    }
}
