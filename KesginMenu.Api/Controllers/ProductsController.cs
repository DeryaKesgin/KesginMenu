using Microsoft.AspNetCore.Authorization;
using KesginMenu.Api.DTOs;
using KesginMenu.Api.Exceptions;
using KesginMenu.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KesginMenu.Api.Controllers;

[ApiController]
[Authorize(Roles = "BusinessAdmin")]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<ActionResult<List<ProductDto>>> GetByCategory(
        int categoryId)
    {
        try
        {
            var products =
                await _productService.GetByCategoryIdAsync(categoryId);

            return Ok(products);
        }
        catch (BusinessRuleException exception)
        {
            return StatusCode(
                exception.StatusCode,
                new { message = exception.Message });
        }
    }

    [HttpGet("business/{businessId:int}")]
    public async Task<ActionResult<List<ProductDto>>> GetByBusiness(
        int businessId)
    {
        try
        {
            var products =
                await _productService.GetByBusinessIdAsync(businessId);

            return Ok(products);
        }
        catch (BusinessRuleException exception)
        {
            return StatusCode(
                exception.StatusCode,
                new { message = exception.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound(new
            {
                message = "Ürün bulunamadı."
            });
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        CreateProductDto request)
    {
        try
        {
            var product =
                await _productService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id },
                product);
        }
        catch (BusinessRuleException exception)
        {
            return StatusCode(
                exception.StatusCode,
                new { message = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductDto request)
    {
        try
        {
            await _productService.UpdateAsync(id, request);

            return Ok(new
            {
                message = "Ürün başarıyla güncellendi."
            });
        }
        catch (BusinessRuleException exception)
        {
            return StatusCode(
                exception.StatusCode,
                new { message = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productService.DeleteAsync(id);

            return Ok(new
            {
                message = "Ürün başarıyla silindi."
            });
        }
        catch (BusinessRuleException exception)
        {
            return StatusCode(
                exception.StatusCode,
                new { message = exception.Message });
        }
    }
}