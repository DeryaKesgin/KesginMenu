namespace KesginMenu.Api.DTOs;

public class PublicMenuDto
{
    public int BusinessId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public List<MenuCategoryDto> Categories { get; set; } = [];
}