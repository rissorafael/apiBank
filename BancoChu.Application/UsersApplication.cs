using BancoChu.Application.Dtos.Users;
using BancoChu.Application.Interfaces;
using BancoChu.Domain.Entities;
using BancoChu.Domain.Interfaces;


namespace BancoChu.Application
{
    public class UsersApplication : IUsersApplication
    {
        private readonly IUserRepository _userRepository;

        public UsersApplication(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponseDto> CreateAsync(CreateUserRequestDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser is not null)
                return null; 


            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Status = user.Status
            };
        }

    }
}
