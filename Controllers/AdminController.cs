using EventHub.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.IsActive,
                    u.CreatedAt,
                    EventsOrganized = u.OrganizedEvents.Count,
                    CommentsCount = u.Comments.Count
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] string role)
        {
            var validRoles = new[] { "Admin", "Organizator", "Participant" };
            if (!validRoles.Contains(role))
                return BadRequest("Rol invalid. Foloseste: Admin, Organizator, Participant.");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Userul nu a fost gasit.");

            user.Role = role;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Rolul userului {user.FullName} a fost schimbat la {role}." });
        }

        [HttpPut("users/{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Userul nu a fost gasit.");

            user.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Contul {user.FullName} a fost dezactivat." });
        }

        [HttpPut("users/{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Userul nu a fost gasit.");

            user.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Contul {user.FullName} a fost reactivat." });
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Userul nu a fost gasit.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Contul {user.FullName} a fost sters." });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalEvents = await _context.Events.CountAsync(),
                TotalInvitations = await _context.Invitations.CountAsync(),
                TotalComments = await _context.Comments.CountAsync(),
                UsersByRole = new
                {
                    Admins = await _context.Users.CountAsync(u => u.Role == "Admin"),
                    Organizatori = await _context.Users.CountAsync(u => u.Role == "Organizator"),
                    Participanti = await _context.Users.CountAsync(u => u.Role == "Participant")
                },
                InvitationsByStatus = new
                {
                    Pending = await _context.Invitations.CountAsync(i => i.Status == "Pending"),
                    Accepted = await _context.Invitations.CountAsync(i => i.Status == "Accepted"),
                    Declined = await _context.Invitations.CountAsync(i => i.Status == "Declined")
                }
            };

            return Ok(stats);
        }
    }
}