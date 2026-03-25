namespace EventHub.Models
{
    public class EventRequest
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public Event Event { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}