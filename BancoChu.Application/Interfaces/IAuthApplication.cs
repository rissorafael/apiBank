using BancoChu.Application.Dtos.Auth;

namespace BancoChu.Application.Interfaces
{

    public interface IAuthApplication
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}
