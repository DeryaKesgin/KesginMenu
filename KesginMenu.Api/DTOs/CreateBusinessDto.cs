namespace KesginMenu.Api.DTOs;

public class CreateBusinessDto
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    public string? Description { get; set; }
}