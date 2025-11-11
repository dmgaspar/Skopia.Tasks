using Skopia.Tasks.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainEnums = Skopia.Tasks.Domain.Enums;

namespace Skopia.Tasks.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; } = default!;

        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }

        public DomainEnums.TaskStatus Status { get; set; } = DomainEnums.TaskStatus.Pending;
        public TaskPriority Priority { get; set; }

        public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
        public ICollection<TaskHistory> History { get; set; } = new List<TaskHistory>();
    }
}