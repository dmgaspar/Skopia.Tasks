using Microsoft.AspNetCore.Mvc;
using Skopia.Tasks.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("performance")]
    public async Task<IActionResult> GetPerformanceReport([FromQuery] string role = "manager")
    {
        try
        {
            // Simulate permission check
            if (role.ToLower() != "manager")
                throw new UnauthorizedAccessException("O acesso é permitido apenas para usuários com privilégios de administrador.");

            var report = await _reportService.GetPerformanceReportAsync();
            return Ok(report);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro inesperado.", detail = ex.Message });
        }
    }
}
