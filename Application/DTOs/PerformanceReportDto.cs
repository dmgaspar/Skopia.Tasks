using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skopia.Tasks.Application.DTOs
{
    public class PerformanceReportDto
    {
        public int UserId { get; set; }
        public int CompletedTasks { get; set; }
        public double AveragePerDay { get; set; }
    }
}
