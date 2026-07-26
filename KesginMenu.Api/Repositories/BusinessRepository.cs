using KesginMenu.Api.Data;
using KesginMenu.Api.Entities;
using KesginMenu.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KesginMenu.Api.Repositories;

public class BusinessRepository : IBusinessRepository
{
    private readonly AppDbContext _context;

    public BusinessRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Business>> GetAllAsync()
    {
        return await _context.Businesses
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Business?> GetByIdAsync(int id)
    {
        return await _context.Businesses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _context.Businesses
            .AnyAsync(x => x.Slug == slug);
    }

    public async Task<Business> CreateAsync(Business business)
    {
        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();

        return business;
    }
}