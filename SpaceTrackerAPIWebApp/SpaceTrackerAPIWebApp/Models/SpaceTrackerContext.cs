using Microsoft.EntityFrameworkCore;
using SpaceTrackerApp.Models;

namespace SpaceTrackerAPIWebApp.Models
{
    public class SpaceTrackerContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Favorite> Favorites { get; set; }
        public virtual DbSet<FavoriteHistory> FavoriteHistories { get; set; }

        public SpaceTrackerContext(DbContextOptions<SpaceTrackerContext> options)
            : base(options)
        {
        }
    }
}