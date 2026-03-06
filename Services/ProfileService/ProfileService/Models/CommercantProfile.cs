using System.ComponentModel.DataAnnotations;

namespace ProfileService.Models;

public sealed class CommercantProfile : ProfileBase
{
    [MaxLength(20)]
    public string? Telephone { get; set; }

    // Référence vers le commerce dans BusinessService
    public Guid? CommerceId { get; set; }
}