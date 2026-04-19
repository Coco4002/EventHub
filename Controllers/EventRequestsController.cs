using EventHub.Data;
using EventHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EventHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventRequestsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpPost]
        public async Task<IActionResult> SendRequest([FromBody] int eventId)
        {
            var userId = GetUserId();

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Evenimentul nu a fost gasit.");

            if (ev.OrganizerId == userId)
                return BadRequest("Esti organizatorul acestui eveniment.");

            var existingInvitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.EventId == eventId && i.ParticipantId == userId);
            if (existingInvitation != null)
                return BadRequest("Ai deja o invitatie la acest eveniment.");

            var existingRequest = await _context.EventRequests
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
            if (existingRequest != null)
                return BadRequest("Ai deja o cerere in asteptare.");

            var request = new EventRequest
            {
                EventId = eventId,
                UserId = userId,
                Status = "Pending",
                Message = "",
                RequestedAt = DateTime.UtcNow
            };

            _context.EventRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cerere trimisa.", request.Id });
        }

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Organizator,Admin")]
        public async Task<IActionResult> GetEventRequests(int eventId)
        {
            var userId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Evenimentul nu a fost gasit.");

            if (ev.OrganizerId != userId && userRole != "Admin")
                return Forbid();

            var requests = await _context.EventRequests
                .Where(r => r.EventId == eventId)
                .Include(r => r.User)
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new
                {
                    r.Id,
                    r.Status,
                    r.RequestedAt,
                    r.RespondedAt,
                    User = new { r.User.Id, r.User.FullName, r.User.Email }
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = GetUserId();

            var requests = await _context.EventRequests
                .Where(r => r.UserId == userId)
                .Include(r => r.Event)
                    .ThenInclude(e => e.Category)
                .Include(r => r.Event)
                    .ThenInclude(e => e.Organizer)
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new
                {
                    r.Id,
                    r.Status,
                    r.RequestedAt,
                    r.RespondedAt,
                    Event = new
                    {
                        r.Event.Id,
                        r.Event.Title,
                        r.Event.Location,
                        r.Event.EventDate,
                        Category = new { r.Event.Category.Id, r.Event.Category.Name },
                        Organizer = new { r.Event.Organizer.Id, r.Event.Organizer.FullName }
                    }
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpPut("{id}/respond")]
        [Authorize(Roles = "Organizator,Admin")]
        public async Task<IActionResult> Respond(int id, [FromBody] string status)
        {
            var userId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            if (status != "Approved" && status != "Rejected")
                return BadRequest("Status invalid. Foloseste 'Approved' sau 'Rejected'.");

            var request = await _context.EventRequests
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound("Cererea nu a fost gasita.");

            if (request.Event.OrganizerId != userId && userRole != "Admin")
                return Forbid();

            if (request.Status != "Pending")
                return BadRequest("Ai raspuns deja la aceasta cerere.");

            request.Status = status;
            request.RespondedAt = DateTime.UtcNow;

            if (status == "Approved")
            {
                var invitation = new Invitation
                {
                    EventId = request.EventId,
                    ParticipantId = request.UserId,
                    Status = "Accepted",
                    SentAt = DateTime.UtcNow,
                    RespondedAt = DateTime.UtcNow
                };
                _context.Invitations.Add(invitation);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Cerere {status}." });
        }
    }
}