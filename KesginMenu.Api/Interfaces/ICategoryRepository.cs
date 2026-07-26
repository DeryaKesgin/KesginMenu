using KesginMenu.Api.Entities;

namespace KesginMenu.Api.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetByBusinessIdAsync(int businessId);
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
}