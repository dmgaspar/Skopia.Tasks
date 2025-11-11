using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skopia.Tasks.Domain.Entities
{
    public class TaskHistory
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = default!;
        public string FieldName { get; set; } = default!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public int ChangedByUserId { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}