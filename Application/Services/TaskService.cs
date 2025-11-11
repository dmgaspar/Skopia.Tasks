using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Domain.Entities;
using Skopia.Tasks.Domain.Enums;
using Skopia.Tasks.Infrastructure.Persistence;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Exceptions;
using Skopia.Tasks.Application.Interfaces;

namespace Skopia.Tasks.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<TaskDto>> GetByProjectIdAsync(int projectId)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
                throw new NotFoundException("Project not found");

            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description ?? "",
                    DueDate = t.DueDate,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString()
                })
                .ToListAsync();

            return tasks;
        }

        public async Task<TaskDto> CreateAsync(int projectId, TaskDto dto)
        {
            var project = await _context.Projects.FindAsync(projectId)
                ?? throw new NotFoundException("Project not found");

            var count = await _context.Tasks.CountAsync(t => t.ProjectId == projectId);
            if (count >= 20)
                throw new BusinessException("A project cannot have more than 20 tasks");

            var task = new TaskItem
            {
                ProjectId = projectId,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Priority = Enum.Parse<TaskPriority>(dto.Priority, true),
                Status = Enum.Parse<Skopia.Tasks.Domain.Enums.TaskStatus>(dto.Status, true)
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            dto.Id = task.Id;
            return dto;
        }

        public async Task<TaskDto> UpdateAsync(int id, TaskDto dto)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                throw new NotFoundException("Task not found");

            // Save original values for history tracking
            var originalTitle = task.Title;
            var originalDescription = task.Description;
            var originalDueDate = task.DueDate?.ToString("yyyy-MM-dd");
            var originalStatus = task.Status.ToString();

            // Business rule: cannot change Priority
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.DueDate = dto.DueDate;
            task.Status = Enum.Parse<Domain.Enums.TaskStatus>(dto.Status, true);

            // Add history entries for changes
            AddHistoryIfChanged(task, nameof(TaskItem.Title), originalTitle, task.Title);
            AddHistoryIfChanged(task, nameof(TaskItem.Description), originalDescription, task.Description);
            AddHistoryIfChanged(task, nameof(TaskItem.DueDate), originalDueDate, task.DueDate?.ToString("yyyy-MM-dd"));
            AddHistoryIfChanged(task, nameof(TaskItem.Status), originalStatus, task.Status.ToString());

            await _context.SaveChangesAsync();

            return dto;
        }


        public async Task DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id)
                ?? throw new NotFoundException("Task not found");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        private void AddHistoryIfChanged(TaskItem task, string fieldName, string oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                _context.TaskHistories.Add(new TaskHistory
                {
                    TaskItemId = task.Id,
                    FieldName = fieldName ?? string.Empty,
                    OldValue = oldValue ?? string.Empty,
                    NewValue = newValue,
                    ChangedByUserId = 1, // placeholder for now
                    ChangedAt = DateTime.UtcNow
                });
            }
        }
    }
}
