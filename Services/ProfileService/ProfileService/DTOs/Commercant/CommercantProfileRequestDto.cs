using System.ComponentModel.DataAnnotations;

namespace ProfileService.DTOs.Commercant;

public sealed class CommercantProfileRequestDto
{
    [MaxLength(30)]
    public string? Telephone { get; set; }

    public Guid? CommerceId { get; set; }
}