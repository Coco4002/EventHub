using System;
using System.Collections.Generic;

namespace EventHub.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime EventDate { get; set; }
        public int OrganizerId { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Organizer { get; set; }
        public Category Category { get; set; }
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
        public ICollection<EventRequest> EventRequests { get; set; } = new List<EventRequest>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}