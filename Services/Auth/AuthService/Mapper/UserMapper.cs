using AuthService.Domain.Entities;
using AuthService.DTos;

namespace AuthService.Mapper
{
    public class UserMapper : IUserMapper

    {
        public User ToNewUser(RegisterRequestDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            return new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = "TODO_HASH_LATER",
                Role = "User",
                IsActive = true,
                IsEmailVerified = false,
                CreatedAtUtc = DateTime.UtcNow,
                LastLoginAtUtc = null
            };
        }
}
}
