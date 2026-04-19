using EventHub.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EventHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.CreatedAt,
                EventsOrganized = await _context.Events.CountAsync(e => e.OrganizerId == userId),
                InvitationsReceived = await _context.Invitations.CountAsync(i => i.ParticipantId == userId),
                CommentsCount = await _context.Comments.CountAsync(c => c.UserId == userId)
            });
        }

        [HttpPut("name")]
        public async Task<IActionResult> UpdateName([FromBody] string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return BadRequest("Numele nu poate fi gol.");

            var userId = GetUserId();
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.FullName = fullName.Trim();
            await _context.SaveChangesAsync();

            return Ok(new { message = "Nume actualizat.", user.FullName });
        }

        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = GetUserId();
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return BadRequest("Parola actuala este incorecta.");

            if (dto.NewPassword.Length < 6)
                return BadRequest("Parola noua trebuie sa aiba minim 6 caractere.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Parola actualizata." });
        }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}