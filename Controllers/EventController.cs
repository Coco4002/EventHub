using EventHub.Data;
using EventHub.DTOs.Events;
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
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? categoryId)
        {
            var query = _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Category)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);

            var events = await query
                .OrderByDescending(e => e.EventDate)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                    e.Location,
                    e.EventDate,
                    e.CreatedAt,
                    Category = new { e.Category.Id, e.Category.Name },
                    Organizer = new { e.Organizer.Id, e.Organizer.FullName, e.Organizer.Email },
                    ParticipantCount = e.Invitations.Count(i => i.Status == "Accepted")
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Category)
                .Include(e => e.Invitations).ThenInclude(i => i.Participant)
                .Include(e => e.Comments).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound("Evenimentul nu a fost gasit.");

            return Ok(new
            {
                ev.Id,
                ev.Title,
                ev.Description,
                ev.Location,
                ev.EventDate,
                ev.CreatedAt,
                Category = new { ev.Category.Id, ev.Category.Name },
                Organizer = new { ev.Organizer.Id, ev.Organizer.FullName },
                Invitations = ev.Invitations.Select(i => new
                {
                    i.Id,
                    i.Status,
                    i.SentAt,
                    Participant = new { i.Participant.Id, i.Participant.FullName }
                }),
                Comments = ev.Comments.Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.CreatedAt,
                    User = new { c.User.Id, c.User.FullName }
                })
            });
        }

        [HttpGet("my")]
        [Authorize(Roles = "Organizator,Admin")]
        public async Task<IActionResult> GetMyEvents()
        {
            var userId = GetUserId();

            var events = await _context.Events
                .Where(e => e.OrganizerId == userId)
                .Include(e => e.Category)
                .Include(e => e.Invitations)
                .OrderByDescending(e => e.EventDate)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Location,
                    e.EventDate,
                    Category = new { e.Category.Id, e.Category.Name },
                    TotalInvited = e.Invitations.Count,
                    Accepted = e.Invitations.Count(i => i.Status == "Accepted"),
                    Pending = e.Invitations.Count(i => i.Status == "Pending")
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpPost]
        [Authorize(Roles = "Organizator,Admin")]
        public async Task<IActionResult> Create(CreateEventDto dto)
        {
            var userId = GetUserId();

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null) return BadRequest("Categoria nu exista.");

            var newEvent = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                EventDate = dto.EventDate,
                CategoryId = dto.CategoryId,
                OrganizerId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync(); 

            var interestedUsers = await _context.UserCategories
                .Where(uc => uc.CategoryId == dto.CategoryId && uc.UserId != userId)
                .Select(uc => uc.UserId)
                .ToListAsync();

            var invitations = interestedUsers.Select(participantId => new Invitation
            {
                EventId = newEvent.Id,
                ParticipantId = participantId,
                Status = "Pending",
                SentAt = DateTime.UtcNow
            }).ToList();

            if (invitations.Any())
            {
                _context.Invitations.AddRange(invitations);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetById), new { id = newEvent.Id }, new
            {
                newEvent.Id,
                newEvent.Title,
                newEvent.EventDate,
                InvitationsSent = invitations.Count,
                message = $"Eveniment creat. {invitations.Count} invitatii trimise automat."
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Organizator,Admin")]
        public async Task<IActionResult> Update(int id, UpdateEventDto dto)
        {
            var userId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound("Evenimentul nu a fost gasit.");

            if (ev.OrganizerId != userId && userRole != "Admin")
                return Forbid();

            ev.Title = dto.Title;
            ev.Description = dto.Description;
            ev.Location = dto.Location;
            ev.EventDate = dto.EventDate;
            ev.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Eveniment actualizat." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Organizator,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound("Evenimentul nu a fost gasit.");

            if (ev.OrganizerId != userId && userRole != "Admin")
                return Forbid();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Eveniment sters." });
        }
    }
}