using System.Net.Http.Json;
using ApiGateway.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("gateway")]
public class RegistrationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RegistrationController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("register-complete")]
    public async Task<IActionResult> RegisterComplete([FromBody] RegisterCompleteRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new { message = "Password et ConfirmPassword ne correspondent pas." });
        }

        var authClient = _httpClientFactory.CreateClient("AuthService");
        var profileClient = _httpClientFactory.CreateClient("ProfileService");

        // 1) Appel AuthService
        var authPayload = new
        {
            email = request.Email,
            password = request.Password
        };

        var authResponse = await authClient.PostAsJsonAsync("auth/register", authPayload);

        if (!authResponse.IsSuccessStatusCode)
        {
            var authError = await authResponse.Content.ReadAsStringAsync();
            return StatusCode((int)authResponse.StatusCode, new
            {
                step = "AuthService",
                error = authError
            });
        }

        var authResult = await authResponse.Content.ReadFromJsonAsync<AuthRegisterResponse>();

        if (authResult is null || authResult.UserId == Guid.Empty)
        {
            return StatusCode(500, new
            {
                step = "AuthService",
                error = "Réponse invalide : UserId manquant."
            });
        }

        // 2) Appel ProfileService
        var profilePayload = new ProfileRegisterInitRequest
        {
            UserId = authResult.UserId,
            Prenom = request.Prenom,
            Nom = request.Nom,
            DateNaissance = request.DateNaissance,
            Genre = request.Genre
        };

        var profileResponse = await profileClient.PostAsJsonAsync(
            "internal/touriste/profile/register-init",
            profilePayload);

        if (!profileResponse.IsSuccessStatusCode)
        {
            var profileError = await profileResponse.Content.ReadAsStringAsync();

            return StatusCode((int)profileResponse.StatusCode, new
            {
                step = "ProfileService",
                error = profileError,
                message = "Compte créé, mais profil initial non créé."
            });
        }

        return Ok(new
        {
            message = "Inscription complète réussie."
        });
    }
}