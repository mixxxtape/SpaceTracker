using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceTrackerApp.Models;

namespace SpaceTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly SpaceTrackerContext _context;

        public FavoritesController(SpaceTrackerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var favorites = await _context.Favorites.ToListAsync();
            return Ok(favorites);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null) return NotFound();
            return Ok(favorite);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Favorite favorite)
        {
            favorite.SavedAt = DateTime.UtcNow;
            favorite.UpdatedAt = DateTime.UtcNow;
            favorite.User = null;
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            var history = new FavoriteHistory
            {
                FavoriteId = favorite.Id,
                Action = "Added",
                ChangedAt = DateTime.UtcNow
            };
            _context.FavoriteHistories.Add(history);
            await _context.SaveChangesAsync();

            return Ok(favorite);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Favorite favorite)
        {
            var existing = await _context.Favorites.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = favorite.Title;
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null) return NotFound();

            var history = new FavoriteHistory
            {
                FavoriteId = id,
                Action = "Deleted",
                ChangedAt = DateTime.UtcNow
            };
            _context.FavoriteHistories.Add(history);
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}