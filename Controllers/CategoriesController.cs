using EventHub.Data;
using EventHub.DTOs.Categories;
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
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    EventCount = c.Events.Count,
                    SubscriberCount = c.UserCategories.Count
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyCategories()
        {
            var userId = GetUserId();

            var categories = await _context.UserCategories
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Category)
                .Select(uc => new
                {
                    uc.Category.Id,
                    uc.Category.Name,
                    uc.Category.Description,
                    uc.SubscribedAt
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpPost("{id}/subscribe")]
        public async Task<IActionResult> Subscribe(int id)
        {
            var userId = GetUserId();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound("Categoria nu a fost gasita.");

            var existing = await _context.UserCategories
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CategoryId == id);

            if (existing != null)
                return BadRequest("Esti deja abonat la aceasta categorie.");

            var userCategory = new UserCategory
            {
                UserId = userId,
                CategoryId = id,
                SubscribedAt = DateTime.UtcNow
            };

            _context.UserCategories.Add(userCategory);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Te-ai abonat la categoria '{category.Name}'." });
        }

        [HttpDelete("{id}/unsubscribe")]
        public async Task<IActionResult> Unsubscribe(int id)
        {
            var userId = GetUserId();

            var userCategory = await _context.UserCategories
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CategoryId == id);

            if (userCategory == null)
                return NotFound("Nu esti abonat la aceasta categorie.");

            _context.UserCategories.Remove(userCategory);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Te-ai dezabonat de la categorie." });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Numele categoriei este obligatoriu.");

            var existing = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == dto.Name.ToLower());
            if (existing != null)
                return BadRequest("Aceasta categorie exista deja.");

            var category = new Category
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { category.Id, category.Name });
        }
    }
}