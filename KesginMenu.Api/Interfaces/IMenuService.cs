using KesginMenu.Api.DTOs;

namespace KesginMenu.Api.Interfaces;

public interface IMenuService
{
    Task<PublicMenuDto?> GetPublicMenuAsync(string slug);
}