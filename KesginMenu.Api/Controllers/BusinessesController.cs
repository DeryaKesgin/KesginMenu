using KesginMenu.Api.DTOs;
using KesginMenu.Api.Exceptions;
using KesginMenu.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KesginMenu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessesController : ControllerBase
{
    private readonly IBusinessService _businessService;

    public BusinessesController(IBusinessService businessService)
    {
        _businessService = businessService;
    }

    [HttpGet]
    public async Task<ActionResult<List<BusinessDto>>> GetAll()
    {
        var businesses = await _businessService.GetAllAsync();
        return Ok(businesses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BusinessDto>> GetById(int id)
    {
        var business = await _businessService.GetByIdAsync(id);

        if (business is null)
        {
            return NotFound(new
            {
                message = "İşletme bulunamadı."
            });
        }

        return Ok(business);
    }

    [HttpPost]
    public async Task<ActionResult<BusinessDto>> Create(
        CreateBusinessDto request)
    {
        try
        {
            var business = await _businessService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = business.Id },
                business);
        }
        catch (BusinessRuleException exception)
        {
            return StatusCode(
                exception.StatusCode,
                new { message = exception.Message });
        }
    }
}