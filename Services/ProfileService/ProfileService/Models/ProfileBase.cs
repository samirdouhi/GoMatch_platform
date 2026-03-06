using ProfileService.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProfileService.Models;

public abstract class ProfileBase
{
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [MaxLength(100)]
    public string? Prenom { get; set; }

    [MaxLength(100)]
    public string? Nom { get; set; }

    public DateOnly? DateNaissance { get; set; }

    public Genre? Genre { get; set; }

    [MaxLength(512)]
    public string? PhotoUrl { get; set; }

    public Langue Langue { get; set; } = Langue.FR;

    public bool InscriptionTerminee { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}