using KesginMenu.Api.Data;
using KesginMenu.Api.Entities;
using KesginMenu.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KesginMenu.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.CategoryId == categoryId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<Product>> GetByBusinessIdAsync(int businessId)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.Category.BusinessId == businessId)
            .OrderBy(x => x.Category.DisplayOrder)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        await _context.Entry(product)
            .Reference(x => x.Category)
            .LoadAsync();

        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}