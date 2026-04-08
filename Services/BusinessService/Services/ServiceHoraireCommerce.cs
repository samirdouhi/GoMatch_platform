using BusinessService.DTOs;
using BusinessService.Mappers;
using BusinessService.Repositories;

namespace BusinessService.Services
{
    public class ServiceHoraireCommerce : IServiceHoraireCommerce
    {
        private readonly IHoraireCommerceRepository _repository;

        public ServiceHoraireCommerce(IHoraireCommerceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<HoraireCommerceReponseDto>> ObtenirParCommerceAsync(Guid commerceId)
        {
            var horaires = await _repository.ObtenirParCommerceIdAsync(commerceId);
            return horaires.Select(HoraireCommerceMapper.ToResponse);
        }

        public async Task<HoraireCommerceReponseDto> CreerAsync(Guid commerceId, CreerHoraireCommerceRequeteDto requete)
        {
            var horaire = HoraireCommerceMapper.ToEntity(commerceId, requete);

            await _repository.AjouterAsync(horaire);
            await _repository.SauvegarderAsync();

            return HoraireCommerceMapper.ToResponse(horaire);
        }

        public async Task<HoraireCommerceReponseDto?> ModifierAsync(
            Guid commerceId,
            Guid horaireId,
            ModifierHoraireCommerceRequeteDto requete)
        {
            var horaire = await _repository.ObtenirParIdAsync(horaireId);

            if (horaire == null || horaire.CommerceId != commerceId)
            {
                return null;
            }

            HoraireCommerceMapper.ApplyUpdate(horaire, requete);

            await _repository.MettreAJourAsync(horaire);
            await _repository.SauvegarderAsync();

            return HoraireCommerceMapper.ToResponse(horaire);
        }

        public async Task<bool> SupprimerAsync(Guid commerceId, Guid horaireId)
        {
            var horaire = await _repository.ObtenirParIdAsync(horaireId);

            if (horaire == null || horaire.CommerceId != commerceId)
            {
                return false;
            }

            await _repository.SupprimerAsync(horaire);
            await _repository.SauvegarderAsync();

            return true;
        }
    }
}