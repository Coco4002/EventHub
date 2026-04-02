using EventHub.Data;
using EventHub.DTOs.Comments;
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
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetEventComments(int eventId)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Evenimentul nu a fost gasit.");

            var comments = await _context.Comments
                .Where(c => c.EventId == eventId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.CreatedAt,
                    c.UpdatedAt,
                    User = new { c.User.Id, c.User.FullName }
                })
                .ToListAsync();

            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCommentDto dto)
        {
            var userId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var ev = await _context.Events.FindAsync(dto.EventId);
            if (ev == null) return NotFound("Evenimentul nu a fost gasit.");

            bool canComment = ev.OrganizerId == userId || userRole == "Admin";

            if (!canComment)
            {
                var invitation = await _context.Invitations
                    .FirstOrDefaultAsync(i => i.EventId == dto.EventId
                        && i.ParticipantId == userId
                        && i.Status == "Accepted");

                if (invitation == null)
                    return Forbid(); 
            }

            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Comentariul nu poate fi gol.");

            var comment = new Comment
            {
                EventId = dto.EventId,
                UserId = userId,
                Content = dto.Content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                comment.Id,
                comment.Content,
                comment.CreatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCommentDto dto)
        {
            var userId = GetUserId();

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (comment == null)
                return NotFound("Comentariul nu a fost gasit sau nu iti apartine.");

            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Comentariul nu poate fi gol.");

            comment.Content = dto.Content.Trim();
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Comentariu actualizat.", comment.Content });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var comment = await _context.Comments
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null) return NotFound("Comentariul nu a fost gasit.");

            bool canDelete = comment.UserId == userId
                || comment.Event.OrganizerId == userId
                || userRole == "Admin";

            if (!canDelete) return Forbid();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Comentariu sters." });
        }
    }
}