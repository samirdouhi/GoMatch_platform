using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set;} = default!;

        [Required]
        public string PasswordHash { get; set;}= default!;

        [Required, MaxLength(30)]
        public string Role { get; set; } = "Touriste"; // Touriste | Commerçant | Admin

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
