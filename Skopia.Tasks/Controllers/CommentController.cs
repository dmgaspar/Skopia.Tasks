using Microsoft.AspNetCore.Mvc;
using Skopia.Tasks.Application.Interfaces;

namespace Skopia.Tasks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("{taskItemId}")]
        public async Task<IActionResult> CreateAsync(int taskItemId, [FromBody] string text)
        {
            var created = await _commentService.CreateAsync(taskItemId, text);
            if (created == null)
                return NotFound("Task not found.");

            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] string text)
        {
            var updated = await _commentService.UpdateAsync(id, text);
            if (updated == null)
                return NotFound("Cometário não encontrado");

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _commentService.DeleteAsync(id);
            if (!deleted)
                return NotFound("Cometário não encontrado");

            return NoContent();
        }
    }
}
