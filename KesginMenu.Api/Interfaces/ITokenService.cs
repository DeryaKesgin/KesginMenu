using KesginMenu.Api.DTOs;
using KesginMenu.Api.Entities;

namespace KesginMenu.Api.Interfaces;

public interface ITokenService
{
    LoginResponseDto CreateToken(AppUser user);
}