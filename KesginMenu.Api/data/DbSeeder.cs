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

        var userExists = await context.Users
            .AnyAsync(x => x.Email == demoEmail);

        if (userExists)
        {
            return;
        }

        var businessExists = await context.Businesses
            .AnyAsync(x => x.Id == 1);

        if (!businessExists)
        {
            throw new InvalidOperationException(
                "Demo işletmesi bulunamadı.");
        }

        var user = new AppUser
        {
            FullName = "KesginSoft Demo Kullanıcısı",
            Email = demoEmail,
            Role = "BusinessAdmin",
            BusinessId = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            passwordHasher.HashPassword(user, "Demo123*");

        context.Users.Add(user);
        await context.SaveChangesAsync();
    }
}