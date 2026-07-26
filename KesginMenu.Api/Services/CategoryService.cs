using KesginMenu.Api.DTOs;
using KesginMenu.Api.Entities;
using KesginMenu.Api.Exceptions;
using KesginMenu.Api.Interfaces;

namespace KesginMenu.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBusinessRepository _businessRepository;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IBusinessRepository businessRepository)
    {
        _categoryRepository = categoryRepository;
        _businessRepository = businessRepository;
    }

    public async Task<List<CategoryDto>> GetByBusinessIdAsync(int businessId)
    {
        var business = await _businessRepository.GetByIdAsync(businessId);

        if (business is null)
        {
            throw new BusinessRuleException(
                "İşletme bulunamadı.",
                404);
        }

        var categories =
            await _categoryRepository.GetByBusinessIdAsync(businessId);

        return categories
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        return category is null
            ? null
            : MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(
        CreateCategoryDto request)
    {
        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException(
                "Kategori adı boş olamaz.");
        }

        var business =
            await _businessRepository.GetByIdAsync(request.BusinessId);

        if (business is null)
        {
            throw new BusinessRuleException(
                "Geçerli bir işletme seçilmelidir.",
                404);
        }

        var category = new Category
        {
            Name = name,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            BusinessId = request.BusinessId
        };

        await _categoryRepository.CreateAsync(category);

        return MapToDto(category);
    }

    public async Task UpdateAsync(
        int id,
        UpdateCategoryDto request)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            throw new BusinessRuleException(
                "Kategori bulunamadı.",
                404);
        }

        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException(
                "Kategori adı boş olamaz.");
        }

        category.Name = name;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;

        await _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            throw new BusinessRuleException(
                "Kategori bulunamadı.",
                404);
        }

        await _categoryRepository.DeleteAsync(category);
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            BusinessId = category.BusinessId
        };
    }
}