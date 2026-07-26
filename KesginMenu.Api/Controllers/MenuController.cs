using KesginMenu.Api.DTOs;
using KesginMenu.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KesginMenu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<PublicMenuDto>> GetBySlug(
        string slug)
    {
        var menu = await _menuService.GetPublicMenuAsync(slug);

        if (menu is null)
        {
            return NotFound(new
            {
                message = "Menü bulunamadı."
            });
        }

        return Ok(menu);
    }
}