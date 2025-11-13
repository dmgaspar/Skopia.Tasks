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
                throw new NotFoundException("Projeto não encontrado");

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

        public async Task<TaskDto> CreateAsync(int projectId, TaskCreateDto dto)
        {
            var project = await _context.Projects.FindAsync(projectId)
                ?? throw new NotFoundException("Projeto não encontrado.");

            var count = await _context.Tasks.CountAsync(t => t.ProjectId == projectId);
            if (count >= 20)
                throw new BusinessException("Um projeto não pode ter mais de 20 tarefas.");

            if (!Enum.TryParse<TaskPriority>(dto.Priority, true, out var priority))
                throw new BusinessException($"Prioridade '{dto.Priority}' inválida.");

            if (!Enum.TryParse<Domain.Enums.TaskStatus>(dto.Status, true, out var status))
                throw new BusinessException($"Status '{dto.Status}' inválido.");

            var task = new TaskItem
            {
                ProjectId = projectId,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Priority = priority,
                Status = status
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                ProjectId = projectId,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                Status = task.Status.ToString()
            };
        }

        public async Task<TaskDto> UpdateAsync(int projectId, int taskId, TaskUpdateDto dto)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new NotFoundException("Projeto não encontrado.");

            var task = project.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null)
                throw new NotFoundException("Tarefa não encontrada neste projeto.");

            var originalTitle = task.Title;
            var originalDescription = task.Description;
            var originalDueDate = task.DueDate?.ToString("yyyy-MM-dd");
            var originalStatus = task.Status.ToString();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.DueDate = dto.DueDate;
            task.Status = Enum.Parse<Domain.Enums.TaskStatus>(dto.Status, true);

            AddHistoryIfChanged(task, nameof(TaskItem.Title), originalTitle, task.Title);
            AddHistoryIfChanged(task, nameof(TaskItem.Description), originalDescription, task.Description);
            AddHistoryIfChanged(task, nameof(TaskItem.DueDate), originalDueDate, task.DueDate?.ToString("yyyy-MM-dd"));
            AddHistoryIfChanged(task, nameof(TaskItem.Status), originalStatus, task.Status.ToString());

            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                ProjectId = projectId,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString()
            };
        }


        public async Task DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id)
                ?? throw new NotFoundException("Tarefa não encontrada");

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
