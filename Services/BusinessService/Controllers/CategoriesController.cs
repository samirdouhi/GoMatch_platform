using BusinessService.Models;
using BusinessService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IServiceCategorie _service;

        public CategoriesController(IServiceCategorie service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ObtenirTout()
        {
            var categories = await _service.ObtenirToutAsync();
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenirParId(Guid id)
        {
            var categorie = await _service.ObtenirParIdAsync(id);
            if (categorie == null)
                return NotFound();

            return Ok(categorie);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Creer([FromBody] Categorie categorie)
        {
            var (succes, erreur, resultat) = await _service.CreerAsync(categorie);

            if (!succes)
                return BadRequest(erreur);

            return Ok(resultat);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Modifier(Guid id, [FromBody] Categorie categorieModifiee)
        {
            var resultat = await _service.ModifierAsync(id, categorieModifiee);

            if (resultat == null)
                return NotFound();

            return Ok(resultat);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(Guid id)
        {
            var (succes, erreur) = await _service.SupprimerAsync(id);

            if (erreur != null)
                return BadRequest(erreur);

            if (!succes)
                return NotFound();

            return NoContent();
        }
    }
}