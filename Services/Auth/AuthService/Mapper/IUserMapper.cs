using AuthService.Domain.Entities;
using AuthService.DTos;

namespace AuthService.Mapper
{
    public interface IUserMapper
    {
        User ToNewUser(RegisterRequestDto dto);
    }
}
