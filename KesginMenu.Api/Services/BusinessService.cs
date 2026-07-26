using KesginMenu.Api.DTOs;
using KesginMenu.Api.Entities;
using KesginMenu.Api.Exceptions;
using KesginMenu.Api.Interfaces;

namespace KesginMenu.Api.Services;

public class BusinessService : IBusinessService
{
    private readonly IBusinessRepository _businessRepository;

    public BusinessService(IBusinessRepository businessRepository)
    {
        _businessRepository = businessRepository;
    }

    public async Task<List<BusinessDto>> GetAllAsync()
    {
        var businesses = await _businessRepository.GetAllAsync();

        return businesses.Select(MapToDto).ToList();
    }

    public async Task<BusinessDto?> GetByIdAsync(int id)
    {
        var business = await _businessRepository.GetByIdAsync(id);

        return business is null ? null : MapToDto(business);
    }

    public async Task<BusinessDto> CreateAsync(CreateBusinessDto request)
    {
        var name = request.Name.Trim();
        var slug = NormalizeSlug(request.Slug);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException(
                "İşletme adı boş olamaz.");
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new BusinessRuleException(
                "Menü bağlantı adı boş olamaz.");
        }

        if (await _businessRepository.SlugExistsAsync(slug))
        {
            throw new BusinessRuleException(
                "Bu menü bağlantı adı daha önce kullanılmış.",
                409);
        }

        var business = new Business
        {
            Name = name,
            Slug = slug,
            LogoUrl = NormalizeOptionalText(request.LogoUrl),
            Description = NormalizeOptionalText(request.Description),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _businessRepository.CreateAsync(business);

        return MapToDto(business);
    }

    private static BusinessDto MapToDto(Business business)
    {
        return new BusinessDto
        {
            Id = business.Id,
            Name = business.Name,
            Slug = business.Slug,
            LogoUrl = business.LogoUrl,
            Description = business.Description,
            IsActive = business.IsActive,
            CreatedAt = business.CreatedAt
        };
    }

    private static string NormalizeSlug(string value)
    {
        return value
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "-");
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}