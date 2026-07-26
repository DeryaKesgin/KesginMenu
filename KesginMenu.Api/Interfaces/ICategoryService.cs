using KesginMenu.Api.DTOs;

namespace KesginMenu.Api.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetByBusinessIdAsync(int businessId);
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto request);
    Task UpdateAsync(int id, UpdateCategoryDto request);
    Task DeleteAsync(int id);
}