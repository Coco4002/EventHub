namespace EventHub.DTOs.Comments
{
    public class CreateCommentDto
    {
        public int EventId { get; set; }
        public string Content { get; set; }
    }
}
