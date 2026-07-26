using KesginMenu.Api.Entities;

namespace KesginMenu.Api.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetByCategoryIdAsync(int categoryId);
    Task<List<Product>> GetByBusinessIdAsync(int businessId);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}