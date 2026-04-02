using EventHub.Data;
using EventHub.DTOs.Invitations;
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
    public class InvitationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvitationsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet("my")]
        public async Task<IActionResult> GetMyInvitations()
        {
            var userId = GetUserId();

            var invitations = await _context.Invitations
                .Where(i => i.ParticipantId == userId)
                .Include(i => i.Event)
                    .ThenInclude(e => e.Organizer)
                .Include(i => i.Event)
                    .ThenInclude(e => e.Category)
                .OrderByDescending(i => i.SentAt)
                .Select(i => new
                {
                    i.Id,
                    i.Status,
                    i.SentAt,
                    i.RespondedAt,
                    Event = new
                    {
                        i.Event.Id,
                        i.Event.Title,
                        i.Event.Description,
                        i.Event.Location,
                        i.Event.EventDate,
                        Category = new { i.Event.Category.Id, i.Event.Category.Name },
                        Organizer = new { i.Event.Organizer.Id, i.Event.Organizer.FullName }
                    }
                })
                .ToListAsync();

            return Ok(invitations);
        }

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Organizator,Admin")]
        public async Task<IActionResult> GetEventInvitations(int eventId)
        {
            var userId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Evenimentul nu a fost gasit.");

            if (ev.OrganizerId != userId && userRole != "Admin")
                return Forbid();

            var invitations = await _context.Invitations
                .Where(i => i.EventId == eventId)
                .Include(i => i.Participant)
                .OrderByDescending(i => i.SentAt)
                .Select(i => new
                {
                    i.Id,
                    i.Status,
                    i.SentAt,
                    i.RespondedAt,
                    Participant = new { i.Participant.Id, i.Participant.FullName, i.Participant.Email }
                })
                .ToListAsync();

            return Ok(invitations);
        }

        [HttpPost]
        [Authorize(Roles = "Organizator,Admin")]
        public async Task<IActionResult> SendInvitation(SendInvitationDto dto)
        {
            var userId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var ev = await _context.Events.FindAsync(dto.EventId);
            if (ev == null) return NotFound("Evenimentul nu a fost gasit.");

            if (ev.OrganizerId != userId && userRole != "Admin")
                return Forbid();

            var participant = await _context.Users.FindAsync(dto.ParticipantId);
            if (participant == null) return NotFound("Utilizatorul nu a fost gasit.");

            var existing = await _context.Invitations
                .FirstOrDefaultAsync(i => i.EventId == dto.EventId && i.ParticipantId == dto.ParticipantId);
            if (existing != null)
                return BadRequest("Utilizatorul a fost deja invitat la acest eveniment.");

            var invitation = new Invitation
            {
                EventId = dto.EventId,
                ParticipantId = dto.ParticipantId,
                Status = "Pending",
                SentAt = DateTime.UtcNow
            };

            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Invitatie trimisa.", invitation.Id });
        }

        [HttpPut("{id}/respond")]
        public async Task<IActionResult> Respond(int id, RespondInvitationDto dto)
        {
            var userId = GetUserId();

            if (dto.Status != "Accepted" && dto.Status != "Declined")
                return BadRequest("Status invalid. Foloseste 'Accepted' sau 'Declined'.");

            var invitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.Id == id && i.ParticipantId == userId);

            if (invitation == null)
                return NotFound("Invitatia nu a fost gasita sau nu iti apartine.");

            if (invitation.Status != "Pending")
                return BadRequest("Ai raspuns deja la aceasta invitatie.");

            invitation.Status = dto.Status;
            invitation.RespondedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Invitatie {dto.Status}.", invitation.Status });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Organizator,Admin")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var invitation = await _context.Invitations
                .Include(i => i.Event)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invitation == null) return NotFound("Invitatia nu a fost gasita.");

            if (invitation.Event.OrganizerId != userId && userRole != "Admin")
                return Forbid();

            _context.Invitations.Remove(invitation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Invitatie anulata." });
        }
    }
}