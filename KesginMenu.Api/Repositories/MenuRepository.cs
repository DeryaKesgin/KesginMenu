using KesginMenu.Api.Data;
using KesginMenu.Api.Entities;
using KesginMenu.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KesginMenu.Api.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly AppDbContext _context;

    public MenuRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Business?> GetPublicMenuBySlugAsync(string slug)
    {
        return await _context.Businesses
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.Slug == slug && x.IsActive)
            .Include(x => x.Categories
                .Where(category => category.IsActive))
            .ThenInclude(category => category.Products
                .Where(product => product.IsAvailable))
            .FirstOrDefaultAsync();
    }
}