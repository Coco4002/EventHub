namespace EventHub.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Event Event { get; set; }
        public User User { get; set; }
    }
}