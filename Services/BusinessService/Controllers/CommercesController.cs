using BusinessService.DTOs;
using BusinessService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusinessService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommercesController : ControllerBase
    {
        private readonly IServiceCommerce _service;

        public CommercesController(IServiceCommerce service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ObtenirTout()
        {
            var commerces = await _service.ObtenirToutAsync();
            return Ok(commerces);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenirParId(Guid id)
        {
            var commerce = await _service.ObtenirParIdAsync(id);

            if (commerce == null)
                return NotFound();

            return Ok(commerce);
        }

        // Un utilisateur connecté peut soumettre un commerce
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Creer([FromBody] CreerCommerceRequeteDto requete)
        {
            var proprietaireUtilisateurId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var resultat = await _service.CreerAsync(requete, proprietaireUtilisateurId);

            return Ok(resultat);
        }

        // Pour l’instant : utilisateur connecté
        // Plus tard : owner/admin dans le service
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Modifier(Guid id, [FromBody] ModifierCommerceRequeteDto requete)
        {
            var resultat = await _service.ModifierAsync(id, requete);

            if (resultat == null)
                return NotFound();

            return Ok(resultat);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(Guid id)
        {
            var succes = await _service.SupprimerAsync(id);

            if (!succes)
                return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpPost("{commerceId}/tags")]
        public async Task<IActionResult> AjouterTags(Guid commerceId, [FromBody] List<Guid> tagIds)
        {
            var resultat = await _service.AjouterTagsAsync(commerceId, tagIds);

            if (resultat == null)
                return NotFound("Commerce introuvable");

            return Ok(resultat);
        }

        [AllowAnonymous]
        [HttpGet("proches")]
        public async Task<IActionResult> ObtenirProches(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double rayonKm)
        {
            if (rayonKm <= 0)
                return BadRequest("Le rayon doit être supérieur à 0.");

            var resultat = await _service.ObtenirCommercesProchesAsync(latitude, longitude, rayonKm);

            return Ok(resultat);
        }

        [AllowAnonymous]
        [HttpGet("recherche")]
        public async Task<IActionResult> Rechercher(
            [FromQuery] string? nom,
            [FromQuery] string? categorie,
            [FromQuery] string? tag,
            [FromQuery] bool? estValide)
        {
            if (nom == null && categorie == null && tag == null && estValide == null)
                return BadRequest("Au moins un filtre est requis.");

            var resultat = await _service.RechercherAsync(nom, categorie, tag, estValide);

            return Ok(resultat);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/valider")]
        public async Task<IActionResult> Valider(Guid id)
        {
            var resultat = await _service.ValiderAsync(id);

            if (resultat == null)
                return NotFound();

            return Ok(resultat);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/rejeter")]
        public async Task<IActionResult> Rejeter(Guid id)
        {
            var resultat = await _service.RejeterAsync(id);

            if (resultat == null)
                return NotFound();

            return Ok(resultat);
        }
    }
}