using Microsoft.AspNetCore.Authorization;
using KesginMenu.Api.DTOs;
using KesginMenu.Api.Exceptions;
using KesginMenu.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KesginMenu.Api.Controllers;

[ApiController]
[Authorize(Roles = "BusinessAdmin")]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(
        ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("business/{businessId:int}")]
    public async Task<ActionResult<List<CategoryDto>>> GetByBusiness(
        int businessId)
    {
        try
        {
            var categories =
                await _categoryService.GetByBusinessIdAsync(businessId);

            return Ok(categories);
        }
        catch (BusinessRuleException exception)
        {
            return StatusCode(
                exception.StatusCode,
                new { message = exception.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category is null)
        {
            return NotFound(new
            {
                message = "Kategori bulunamadı."
            });
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(
        CreateCategoryDto request)
    {
        try
        {
            var category =
                await _categoryService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = category.Id },
                category);
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
        UpdateCategoryDto request)
    {
        try
        {
            await _categoryService.UpdateAsync(id, request);

            return Ok(new
            {
                message = "Kategori başarıyla güncellendi."
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
            await _categoryService.DeleteAsync(id);

            return Ok(new
            {
                message = "Kategori başarıyla silindi."
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