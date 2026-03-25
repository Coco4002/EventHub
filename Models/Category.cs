namespace EventHub.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<UserCategory> UserCategories { get; set; } = new List<UserCategory>();
    }
}