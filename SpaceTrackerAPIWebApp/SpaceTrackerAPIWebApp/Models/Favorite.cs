using System.ComponentModel.DataAnnotations;

namespace SpaceTrackerApp.Models
{
    public class Favorite
    {
        public Favorite()
        {
            History = new List<FavoriteHistory>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Назва фото")]
        public string Title { get; set; }

        public string ImageUrl { get; set; }
        public string NasaDate { get; set; }
        public DateTime SavedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<FavoriteHistory> History { get; set; }
    }
}