
using BancoChu.Application.Dtos.Users;

namespace BancoChu.Application.Interfaces
{
    public interface IUsersApplication
    {
        Task<UserResponseDto> CreateAsync(CreateUserRequestDto dto);
    }
}
