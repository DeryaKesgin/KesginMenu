namespace KesginMenu.Api.Entities;

public class AppUser
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "BusinessAdmin";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int BusinessId { get; set; }

    public Business Business { get; set; } = null!;
}