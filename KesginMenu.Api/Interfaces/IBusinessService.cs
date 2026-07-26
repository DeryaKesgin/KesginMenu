using KesginMenu.Api.DTOs;

namespace KesginMenu.Api.Interfaces;

public interface IBusinessService
{
    Task<List<BusinessDto>> GetAllAsync();
    Task<BusinessDto?> GetByIdAsync(int id);
    Task<BusinessDto> CreateAsync(CreateBusinessDto request);
}