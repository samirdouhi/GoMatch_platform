using BusinessService.Models;
using BusinessService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsCulturelsController : ControllerBase
    {
        private readonly IServiceTagCulturel _service;

        public TagsCulturelsController(IServiceTagCulturel service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagCulturel>>> ObtenirTout()
        {
            var tagsCulturels = await _service.ObtenirToutAsync();
            return Ok(tagsCulturels);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<TagCulturel>> ObtenirParId(Guid id)
        {
            var tagCulturel = await _service.ObtenirParIdAsync(id);

            if (tagCulturel == null)
                return NotFound();

            return Ok(tagCulturel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TagCulturel>> Creer([FromBody] TagCulturel tagCulturel)
        {
            var (succes, erreur, resultat) = await _service.CreerAsync(tagCulturel);

            if (!succes)
                return BadRequest(erreur);

            return CreatedAtAction(nameof(ObtenirParId), new { id = resultat!.Id }, resultat);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Modifier(Guid id, [FromBody] TagCulturel tagCulturelModifie)
        {
            var (succes, erreur) = await _service.ModifierAsync(id, tagCulturelModifie);

            if (erreur != null)
                return BadRequest(erreur);

            if (!succes)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(Guid id)
        {
            var succes = await _service.SupprimerAsync(id);

            if (!succes)
                return NotFound();

            return NoContent();
        }
    }
}