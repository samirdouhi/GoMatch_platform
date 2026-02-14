using AuthService.DTos;
using AuthService.Interfaces;
using AuthService.Mapper;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        // Plus tard on injectera: repository + password hasher + token service
        private readonly IUserRepository _userRepository;
        private readonly IUserMapper _userMapper;

        public UserService(IUserRepository userRepository, IUserMapper userMapper)
        {
            _userRepository = userRepository;
            _userMapper = userMapper;
        }


        public async Task<Guid> RegisterAsync(RegisterRequestDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var existing = await _userRepository.GetByEmailAsync(email);
            if (existing != null)
                throw new InvalidOperationException("Email already exists");

            var user = _userMapper.ToNewUser(dto);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user.Id;
        }


        public Task<string> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
        {
            // TODO (étapes suivantes):
            // 1) Chercher user par email
            // 2) Vérifier password (hash)
            // 3) UPDATE LastLoginAtUtc
            // 4) Générer JWT et retourner le token

            return Task.FromResult("TODO_TOKEN");
        }
    }
}
