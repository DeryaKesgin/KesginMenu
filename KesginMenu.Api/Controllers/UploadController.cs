using KesginMenu.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KesginMenu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "BusinessAdmin")]
public class UploadController : ControllerBase
{
    private readonly IImageService _imageService;

    public UploadController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new
            {
                message = "Dosya seçilmedi."
            });
        }

        try
        {
            var imageUrl =
                await _imageService.UploadImageAsync(file);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return BadRequest(new
                {
                    message = "Görsel bağlantısı oluşturulamadı."
                });
            }

            return Ok(new
            {
                imageUrl
            });
        }
        catch (Exception exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }
}