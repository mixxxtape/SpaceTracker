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

        // GET api/favorites
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var favorites = await _context.Favorites.ToListAsync();
            return Ok(favorites);
        }

        // GET api/favorites/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null) return NotFound();
            return Ok(favorite);
        }

        // POST api/favorites
        [HttpPost]
        public async Task<IActionResult> Add(Favorite favorite)
        {
            favorite.SavedAt = DateTime.Now;
            favorite.UpdatedAt = DateTime.Now;
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            // Зберігаємо в історію
            var history = new FavoriteHistory
            {
                FavoriteId = favorite.Id,
                Action = "Added",
                ChangedAt = DateTime.Now
            };
            _context.FavoriteHistories.Add(history);
            await _context.SaveChangesAsync();

            return Ok(favorite);
        }

        // PUT api/favorites/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Favorite favorite)
        {
            var existing = await _context.Favorites.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = favorite.Title;
            existing.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // DELETE api/favorites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null) return NotFound();

            var history = new FavoriteHistory
            {
                FavoriteId = id,
                Action = "Deleted",
                ChangedAt = DateTime.Now
            };
            _context.FavoriteHistories.Add(history);
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}