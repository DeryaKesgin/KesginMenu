using KesginMenu.Api.Entities;

namespace KesginMenu.Api.Interfaces;

public interface IAuthRepository
{
    Task<AppUser?> GetByEmailAsync(string email);
    Task<AppUser> CreateAsync(AppUser user);
}