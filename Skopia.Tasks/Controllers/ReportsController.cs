using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Infrastructure.Persistence;

namespace Skopia.Tasks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("performance")]
        public async Task<IActionResult> GetPerformanceReport([FromQuery] string role = "user")
        {
            if (role.ToLower() != "manager")
                return Forbid("Access is permitted only for users with manager privileges.");

            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var completedTasksQuery = _context.Tasks
                .Where(t =>
                    t.Status == Domain.Enums.TaskStatus.Done &&
                    t.DueDate >= thirtyDaysAgo)
                .GroupBy(t => t.ProjectId) // future change: group by ChangedByUserId
                .Select(g => new PerformanceReportDto
                {
                    UserId = 1, // placeholder for now
                    CompletedTasks = g.Count(),
                    AveragePerDay = Math.Round(g.Count() / 30.0, 2)
                });

            var report = await completedTasksQuery.ToListAsync();

            return Ok(report);
        }
    }

}
