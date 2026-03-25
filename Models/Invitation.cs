namespace EventHub.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int ParticipantId { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime SentAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public Event Event { get; set; }
        public User Participant { get; set; }
    }
}