using System;
using System.Collections.Generic;

namespace EventHub.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
        public ICollection<UserCategory> UserCategories { get; set; } = new List<UserCategory>();
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<EventRequest> EventRequests { get; set; } = new List<EventRequest>();
    }
}