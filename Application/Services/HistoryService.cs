using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Application.Interfaces;
using Skopia.Tasks.Domain.Entities;
using Skopia.Tasks.Infrastructure.Persistence;

namespace Skopia.Tasks.Application.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly AppDbContext _context;

        public HistoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskHistory>> GetAllAsync()
        {
            return await _context.TaskHistories
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }

        public async Task<List<TaskHistory>> GetByTaskAsync(int taskItemId)
        {
            return await _context.TaskHistories
                .Where(h => h.TaskItemId == taskItemId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }
    }
}
