namespace AuthService.Domain.Entities
{
    public class User
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        // Login / identité
        public string Email { get; set; } = string.Empty;

        // Sécurité : on stocke un hash, jamais le mot de passe
        public string PasswordHash { get; set; } = string.Empty;

        // Autorisation simple (on passera aux claims/policies plus tard)
        public string Role { get; set; } = "User";

        // États sécurité
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;

        // Audit
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAtUtc { get; set; }
    }
}
