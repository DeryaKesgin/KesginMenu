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
        const string customerEmail = "demo1@kesginsoft.com";

        // Genel demo işletmesi var mı?
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

        // Genel demo kullanıcı var mı?
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

        // Yeni müşteri işletmesi var mı?
        var customerBusiness = await context.Businesses
            .FirstOrDefaultAsync(x => x.Slug == "deneme-isletmesi");

        if (customerBusiness is null)
        {
            customerBusiness = new Business
            {
                Name = "Deneme İşletmesi",
                Slug = "deneme-isletmesi",
                Description = "3 günlük ücretsiz QR menü demo işletmesi",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Businesses.Add(customerBusiness);
            await context.SaveChangesAsync();
        }

        // Yeni müşteri kullanıcısı var mı?
        var customerUserExists = await context.Users
            .AnyAsync(x => x.Email == customerEmail);

        if (!customerUserExists)
        {
            var customerUser = new AppUser
            {
                FullName = "Deneme İşletmesi Yetkilisi",
                Email = customerEmail,
                Role = "BusinessAdmin",
                BusinessId = customerBusiness.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            customerUser.PasswordHash =
                passwordHasher.HashPassword(
                    customerUser,
                    "KesginDemo2026*");

            context.Users.Add(customerUser);
            await context.SaveChangesAsync();
        }
    }
}