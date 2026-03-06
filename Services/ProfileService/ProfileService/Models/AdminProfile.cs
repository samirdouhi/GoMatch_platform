using System.ComponentModel.DataAnnotations;

namespace ProfileService.Models;

public class AdminProfile : ProfileBase
{
    [MaxLength(100)]
    public string? Departement { get; set; }
}
