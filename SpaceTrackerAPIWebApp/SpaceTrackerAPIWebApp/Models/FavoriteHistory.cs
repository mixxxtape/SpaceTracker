namespace SpaceTrackerApp.Models
{
    public class FavoriteHistory
    {
        public int Id { get; set; }
        public int FavoriteId { get; set; }

        public string Action { get; set; } // "Added" або "Deleted"
        public DateTime ChangedAt { get; set; }

        public virtual Favorite Favorite { get; set; }
    }
}