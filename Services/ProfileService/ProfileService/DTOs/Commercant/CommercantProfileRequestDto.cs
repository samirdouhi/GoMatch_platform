using System.ComponentModel.DataAnnotations;

namespace ProfileService.DTOs.Commercant;

public sealed class CommercantProfileRequestDto
{
    [Required]
    [Phone]
    public string Telephone { get; set; } = default!;

    public Guid? CommerceId { get; set; }
}