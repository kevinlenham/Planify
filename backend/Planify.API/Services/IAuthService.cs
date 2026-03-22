using Planify.API.Models;
using Planify.API.DTOs;

namespace Planify.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Register(RegisterDto dto);
        Task<AuthResponseDto> Login(LoginDto dto);
        Task<User> UpdateUser(int id, UpdateUserDto dto);
        Task<User> ChangeUserPassword(int id, ChangePasswordDto dto);
        Task<User> DeleteUser(int id);
    }
}
