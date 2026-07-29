using KesginMenu.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KesginMenu.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        PasswordHasher<AppUser> passwordHasher)
    {
        const string demoEmail = "demo@kesginsoft.com";

        // Demo işletmesi var mı?
        var business = await context.Businesses
            .FirstOrDefaultAsync(x => x.Slug == "demo-restoran");

        if (business is null)
        {
            business = new Business
            {
                Name = "KesginSoft Demo Restoran",
                Slug = "demo-restoran",
                Description = "QR menü yönetim paneli genel demo işletmesi",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Businesses.Add(business);
            await context.SaveChangesAsync();
        }

        // Demo kullanıcı var mı?
        var userExists = await context.Users
            .AnyAsync(x => x.Email == demoEmail);

        if (!userExists)
        {
            var user = new AppUser
            {
                FullName = "KesginSoft Demo Kullanıcısı",
                Email = demoEmail,
                Role = "BusinessAdmin",
                BusinessId = business.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash =
                passwordHasher.HashPassword(user, "Demo123*");

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}