using Microsoft.AspNetCore.Mvc;
using Skopia.Tasks.Infrastructure.Persistence;
using Skopia.Tasks.Domain.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Skopia.Tasks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{taskItemId}")]
        public async Task<IActionResult> CreateAsync(int taskItemId, [FromBody] string text)
        {
            var task = await _context.Tasks.FindAsync(taskItemId);
            if (task == null)
                return NotFound("Task not found");

            var comment = new TaskComment
            {
                TaskItemId = taskItemId,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };

            _context.TaskComments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] string text)
        {
            var comment = await _context.TaskComments.FindAsync(id);
            if (comment == null)
                return NotFound("Comment not found");

            comment.Text = text;
            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var comment = await _context.TaskComments.FindAsync(id);
            if (comment == null)
                return NotFound("Comment not found");

            _context.TaskComments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
