using System.ComponentModel.DataAnnotations;
using ProfileService.Enums;

namespace ProfileService.DTOs.Common;

public sealed class InitProfileRequestDto
{
    [Required]
    public Guid UserId { get; set; }

  
}