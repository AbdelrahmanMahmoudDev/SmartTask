namespace SmartTask.Api.DTOs
{
    public class AddCommentDTO
    {
        public int TaskId { get; set; }
        public string AuthorId { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}