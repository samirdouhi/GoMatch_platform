using BusinessService.DTOs;

namespace BusinessService.Services
{
    public interface IServiceHoraireCommerce
    {
        Task<IEnumerable<HoraireCommerceReponseDto>> ObtenirParCommerceAsync(Guid commerceId);
        Task<HoraireCommerceReponseDto> CreerAsync(Guid commerceId, CreerHoraireCommerceRequeteDto requete);
        Task<HoraireCommerceReponseDto?> ModifierAsync(Guid commerceId, Guid horaireId, ModifierHoraireCommerceRequeteDto requete);
        Task<bool> SupprimerAsync(Guid commerceId, Guid horaireId);
    }
}