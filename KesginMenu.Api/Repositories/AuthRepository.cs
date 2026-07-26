using KesginMenu.Api.Data;
using KesginMenu.Api.Entities;
using KesginMenu.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KesginMenu.Api.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<AppUser> CreateAsync(AppUser user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}