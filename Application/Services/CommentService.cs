using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Application.DTOs;
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
            var task = await _context.Tasks.FindAsync(taskItemId);
            if (task == null)
                return null;

            var comment = new TaskComment
            {
                TaskItemId = taskItemId,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };

            _context.TaskComments.Add(comment);
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
