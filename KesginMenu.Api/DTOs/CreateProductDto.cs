namespace KesginMenu.Api.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public int CategoryId { get; set; }
}