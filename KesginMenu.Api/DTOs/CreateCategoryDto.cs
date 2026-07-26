namespace KesginMenu.Api.DTOs;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public int BusinessId { get; set; }
}