using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Exceptions;
using Skopia.Tasks.Application.Interfaces;
using Skopia.Tasks.Domain.Entities;
using Skopia.Tasks.Infrastructure.Persistence;

namespace Skopia.Tasks.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CommentDto?> CreateAsync(int taskItemId, string text)
        {
            // Verifica se a tarefa existe
            var task = await _context.Tasks
                .Include(t => t.History)
                .FirstOrDefaultAsync(t => t.Id == taskItemId);

            if (task == null)
                throw new NotFoundException("Tarefa não encontrada.");

            // Cria o comentário
            var comment = new TaskComment
            {
                TaskItemId = taskItemId,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };

            _context.TaskComments.Add(comment);

            var historyEntry = new TaskHistory
            {
                TaskItem = task,
                FieldName = "Comentário",
                OldValue = string.Empty,
                NewValue = text,
                ChangedAt = DateTime.UtcNow,
            };

            _context.TaskHistories.Add(historyEntry);

            await _context.SaveChangesAsync();

            return new CommentDto
            {
                Id = comment.Id,
                TaskItemId = comment.TaskItemId,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt
            };
        }


        public async Task<CommentDto?> UpdateAsync(int id, string text)
        {
            var comment = await _context.TaskComments.FindAsync(id);
            if (comment == null)
                return null;

            comment.Text = text;
            await _context.SaveChangesAsync();

            return new CommentDto
            {
                Id = comment.Id,
                TaskItemId = comment.TaskItemId,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var comment = await _context.TaskComments.FindAsync(id);
            if (comment == null)
                return false;

            _context.TaskComments.Remove(comment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
