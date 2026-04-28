using System.ComponentModel.DataAnnotations;

namespace SpaceTrackerApp.Models
{
    public class User
    {
        public User()
        {
            Favorites = new List<Favorite>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Ім'я користувача")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Favorite> Favorites { get; set; }
    }
}