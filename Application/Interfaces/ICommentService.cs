using Skopia.Tasks.Application.DTOs;

namespace Skopia.Tasks.Application.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDto?> CreateAsync(int taskItemId, string text);
        Task<CommentDto?> UpdateAsync(int id, string text);
        Task<bool> DeleteAsync(int id);
    }
}
