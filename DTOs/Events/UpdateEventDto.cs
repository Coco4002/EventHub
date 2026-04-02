namespace EventHub.DTOs.Events
{
    public class UpdateEventDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime EventDate { get; set; }
        public int CategoryId { get; set; }
    }
}
