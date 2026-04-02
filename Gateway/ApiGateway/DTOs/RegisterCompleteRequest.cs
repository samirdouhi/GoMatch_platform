using System.ComponentModel.DataAnnotations;

namespace ApiGateway.DTOs;

public class RegisterCompleteRequest
{
    [Required]
    [MaxLength(100)]
    public string Prenom { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = default!;

    [Required]
    public DateTime DateNaissance { get; set; }

    [Required]
    public string Genre { get; set; } = default!;

    [MaxLength(100)]
    public string? Nationalite { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = default!;

    [Required]
    public string ConfirmPassword { get; set; } = default!;
}