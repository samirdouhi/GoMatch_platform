using BusinessService.Models;
using BusinessService.Repositories;

namespace BusinessService.Services
{
    public class ServiceCategorie : IServiceCategorie
    {
        private readonly ICategorieRepository _repository;

        public ServiceCategorie(ICategorieRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Categorie>> ObtenirToutAsync()
        {
            return await _repository.ObtenirToutAsync();
        }

        public async Task<Categorie?> ObtenirParIdAsync(Guid id)
        {
            return await _repository.ObtenirParIdAsync(id);
        }

        public async Task<(bool succes, string? erreur, Categorie? resultat)> CreerAsync(Categorie categorie)
        {
            var existe = await _repository.ExisteParNomAsync(categorie.Nom);
            if (existe)
            {
                return (false, "Une catégorie avec ce nom existe déjà.", null);
            }

            categorie.Id = Guid.NewGuid();

            await _repository.AjouterAsync(categorie);
            await _repository.SauvegarderAsync();

            return (true, null, categorie);
        }

        public async Task<Categorie?> ModifierAsync(Guid id, Categorie categorieModifiee)
        {
            var categorie = await _repository.ObtenirParIdAsync(id);
            if (categorie == null)
            {
                return null;
            }

            categorie.Nom = categorieModifiee.Nom;

            await _repository.SauvegarderAsync();

            return categorie;
        }

        public async Task<(bool succes, string? erreur)> SupprimerAsync(Guid id)
        {
            var categorie = await _repository.ObtenirParIdAsync(id);
            if (categorie == null)
            {
                return (false, null);
            }

            if (categorie.Commerces.Any())
            {
                return (false, "Impossible de supprimer une catégorie liée à des commerces.");
            }

            await _repository.SupprimerAsync(categorie);
            await _repository.SauvegarderAsync();

            return (true, null);
        }
    }
}