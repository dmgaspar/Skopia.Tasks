using Skopia.Tasks.Application.DTOs;

namespace Skopia.Tasks.Application.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<PerformanceReportDto>> GetPerformanceReportAsync();
    }
}
