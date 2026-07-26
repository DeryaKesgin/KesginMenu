namespace KesginMenu.Api.Entities;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public int BusinessId { get; set; }

    public Business Business { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}