using KesginMenu.Api.DTOs;
using KesginMenu.Api.Interfaces;

namespace KesginMenu.Api.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<PublicMenuDto?> GetPublicMenuAsync(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var normalizedSlug = slug.Trim().ToLowerInvariant();

        var business =
            await _menuRepository.GetPublicMenuBySlugAsync(normalizedSlug);

        if (business is null)
        {
            return null;
        }

        return new PublicMenuDto
        {
            BusinessId = business.Id,
            BusinessName = business.Name,
            Slug = business.Slug,
            LogoUrl = business.LogoUrl,
            Description = business.Description,

            Categories = business.Categories
                .OrderBy(category => category.DisplayOrder)
                .ThenBy(category => category.Name)
                .Select(category => new MenuCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    DisplayOrder = category.DisplayOrder,

                    Products = category.Products
                        .OrderBy(product => product.DisplayOrder)
                        .ThenBy(product => product.Name)
                        .Select(product => new MenuProductDto
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            ImageUrl = product.ImageUrl,
                            DisplayOrder = product.DisplayOrder
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}