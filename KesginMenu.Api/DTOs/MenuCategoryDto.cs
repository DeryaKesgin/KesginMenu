namespace KesginMenu.Api.DTOs;

public class MenuCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public List<MenuProductDto> Products { get; set; } = [];
}