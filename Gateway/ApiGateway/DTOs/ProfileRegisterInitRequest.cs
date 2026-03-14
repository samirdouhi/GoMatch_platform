namespace ApiGateway.DTOs;

public class ProfileRegisterInitRequest
{
    public Guid UserId { get; set; }
    public string Prenom { get; set; } = default!;
    public string Nom { get; set; } = default!;
    public DateTime DateNaissance { get; set; }
    public string Genre { get; set; } = default!;
}