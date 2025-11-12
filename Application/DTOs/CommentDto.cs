namespace Skopia.Tasks.Application.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
