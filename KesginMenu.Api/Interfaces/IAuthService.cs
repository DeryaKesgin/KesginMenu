using KesginMenu.Api.DTOs;

namespace KesginMenu.Api.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto request);
}