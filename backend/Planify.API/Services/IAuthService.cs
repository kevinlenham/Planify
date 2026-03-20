using Planify.API.DTOs;

namespace Planify.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Register(RegisterDto dto);
        Task<AuthResponseDto> Login(LoginDto dto);
    } 
}
