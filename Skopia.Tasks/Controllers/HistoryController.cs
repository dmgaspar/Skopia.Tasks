using Microsoft.AspNetCore.Mvc;
using Skopia.Tasks.Application.Exceptions;
using Skopia.Tasks.Application.Interfaces;
using Skopia.Tasks.Application.Services;

namespace Skopia.Tasks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService service)
        {
            _historyService = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var history = await _historyService.GetAllAsync();
            return Ok(history);
        }

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetByTaskAsync(int taskId)
        {
            try
            {
                var result = await _historyService.GetByTaskAsync(taskId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
