namespace EventHub.Models
{
    public class UserCategory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public DateTime SubscribedAt { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
    }
}