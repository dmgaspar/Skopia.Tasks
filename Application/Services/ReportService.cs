using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Interfaces;
using Skopia.Tasks.Domain.Enums;
using Skopia.Tasks.Infrastructure.Persistence;

namespace Skopia.Tasks.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PerformanceReportDto>> GetPerformanceReportAsync(string role)
        {
            // Simulate permission check
            if (role.ToLower() != "manager")
                throw new UnauthorizedAccessException("Access is permitted only for users with manager privileges.");

            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var completedTasksQuery = _context.Tasks
                .Where(t => t.Status == Domain.Enums.TaskStatus.Done && t.DueDate >= thirtyDaysAgo)
                .GroupBy(t => t.ProjectId) // future: group by ChangedByUserId
                .Select(g => new PerformanceReportDto
                {
                    UserId = 1, // placeholder for now
                    CompletedTasks = g.Count(),
                    AveragePerDay = Math.Round(g.Count() / 30.0, 2)
                });

            return await completedTasksQuery.ToListAsync();
        }
    }
}
