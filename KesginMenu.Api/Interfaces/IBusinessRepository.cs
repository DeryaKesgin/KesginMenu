using KesginMenu.Api.Entities;

namespace KesginMenu.Api.Interfaces;

public interface IBusinessRepository
{
    Task<List<Business>> GetAllAsync();
    Task<Business?> GetByIdAsync(int id);
    Task<bool> SlugExistsAsync(string slug);
    Task<Business> CreateAsync(Business business);
}