using KesginMenu.Api.DTOs;
using KesginMenu.Api.Entities;
using KesginMenu.Api.Exceptions;
using KesginMenu.Api.Interfaces;

namespace KesginMenu.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBusinessRepository _businessRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBusinessRepository businessRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _businessRepository = businessRepository;
    }

    public async Task<List<ProductDto>> GetByCategoryIdAsync(int categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);

        if (category is null)
        {
            throw new BusinessRuleException(
                "Kategori bulunamadı.",
                404);
        }

        var products =
            await _productRepository.GetByCategoryIdAsync(categoryId);

        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetByBusinessIdAsync(int businessId)
    {
        var business = await _businessRepository.GetByIdAsync(businessId);

        if (business is null)
        {
            throw new BusinessRuleException(
                "İşletme bulunamadı.",
                404);
        }

        var products =
            await _productRepository.GetByBusinessIdAsync(businessId);

        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto request)
    {
        ValidateProduct(
            request.Name,
            request.Price);

        var category =
            await _categoryRepository.GetByIdAsync(request.CategoryId);

        if (category is null)
        {
            throw new BusinessRuleException(
                "Geçerli bir kategori seçilmelidir.",
                404);
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = NormalizeOptionalText(request.Description),
            Price = request.Price,
            ImageUrl = NormalizeOptionalText(request.ImageUrl),
            IsAvailable = true,
            DisplayOrder = request.DisplayOrder,
            CategoryId = request.CategoryId
        };

        await _productRepository.CreateAsync(product);

        return MapToDto(product);
    }

    public async Task UpdateAsync(
        int id,
        UpdateProductDto request)
    {
        ValidateProduct(
            request.Name,
            request.Price);

        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            throw new BusinessRuleException(
                "Ürün bulunamadı.",
                404);
        }

        var category =
            await _categoryRepository.GetByIdAsync(request.CategoryId);

        if (category is null)
        {
            throw new BusinessRuleException(
                "Geçerli bir kategori seçilmelidir.",
                404);
        }

        product.Name = request.Name.Trim();
        product.Description =
            NormalizeOptionalText(request.Description);
        product.Price = request.Price;
        product.ImageUrl =
            NormalizeOptionalText(request.ImageUrl);
        product.IsAvailable = request.IsAvailable;
        product.DisplayOrder = request.DisplayOrder;
        product.CategoryId = request.CategoryId;

        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            throw new BusinessRuleException(
                "Ürün bulunamadı.",
                404);
        }

        await _productRepository.DeleteAsync(product);
    }

    private static void ValidateProduct(
        string name,
        decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException(
                "Ürün adı boş olamaz.");
        }

        if (price < 0)
        {
            throw new BusinessRuleException(
                "Ürün fiyatı sıfırdan küçük olamaz.");
        }
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            IsAvailable = product.IsAvailable,
            DisplayOrder = product.DisplayOrder,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty
        };
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}