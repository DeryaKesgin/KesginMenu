using KesginMenu.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace KesginMenu.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Business> Businesses => Set<Business>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(x => x.Slug)
                .IsUnique();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(x => x.Business)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Price)
                .HasPrecision(10, 2);

            entity.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AppUser>(entity =>
{
    entity.HasKey(x => x.Id);

    entity.Property(x => x.FullName)
        .IsRequired()
        .HasMaxLength(150);

    entity.Property(x => x.Email)
        .IsRequired()
        .HasMaxLength(200);

    entity.Property(x => x.PasswordHash)
        .IsRequired();

    entity.Property(x => x.Role)
        .IsRequired()
        .HasMaxLength(50);

    entity.HasIndex(x => x.Email)
        .IsUnique();

    entity.HasOne(x => x.Business)
        .WithMany(x => x.Users)
        .HasForeignKey(x => x.BusinessId)
        .OnDelete(DeleteBehavior.Cascade);
});
    }
}