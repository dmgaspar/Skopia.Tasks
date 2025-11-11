using Microsoft.AspNetCore.Mvc;
using Skopia.Tasks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Skopia.Tasks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HistoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var history = await _context.TaskHistories
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();

            return Ok(history);
        }

        [HttpGet("task/{taskItemId}")]
        public async Task<IActionResult> GetByTaskAsync(int taskItemId)
        {
            var history = await _context.TaskHistories
                .Where(h => h.TaskItemId == taskItemId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();

            if (!history.Any())
                return NotFound("No history for this task");

            return Ok(history);
        }
    }
}
