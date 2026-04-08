using BusinessService.DTOs;
using BusinessService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessService.Controllers
{
    [ApiController]
    [Route("api/commerces/{commerceId}/horaires")]
    public class HorairesCommercesController : ControllerBase
    {
        private readonly IServiceHoraireCommerce _service;

        public HorairesCommercesController(IServiceHoraireCommerce service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ObtenirTout(Guid commerceId)
        {
            var resultat = await _service.ObtenirParCommerceAsync(commerceId);
            return Ok(resultat);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Creer(Guid commerceId, [FromBody] CreerHoraireCommerceRequeteDto requete)
        {
            var resultat = await _service.CreerAsync(commerceId, requete);
            return Ok(resultat);
        }

        [Authorize]
        [HttpPut("{horaireId}")]
        public async Task<IActionResult> Modifier(Guid commerceId, Guid horaireId, [FromBody] ModifierHoraireCommerceRequeteDto requete)
        {
            var resultat = await _service.ModifierAsync(commerceId, horaireId, requete);

            if (resultat == null)
                return NotFound();

            return Ok(resultat);
        }

        [Authorize]
        [HttpDelete("{horaireId}")]
        public async Task<IActionResult> Supprimer(Guid commerceId, Guid horaireId)
        {
            var succes = await _service.SupprimerAsync(commerceId, horaireId);

            if (!succes)
                return NotFound();

            return NoContent();
        }
    }
}