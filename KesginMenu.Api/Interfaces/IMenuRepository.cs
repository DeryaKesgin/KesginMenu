using KesginMenu.Api.Entities;

namespace KesginMenu.Api.Interfaces;

public interface IMenuRepository
{
    Task<Business?> GetPublicMenuBySlugAsync(string slug);
}