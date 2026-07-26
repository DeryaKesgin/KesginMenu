using KesginMenu.Api.DTOs;

namespace KesginMenu.Api.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetByCategoryIdAsync(int categoryId);
    Task<List<ProductDto>> GetByBusinessIdAsync(int businessId);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto request);
    Task UpdateAsync(int id, UpdateProductDto request);
    Task DeleteAsync(int id);
}