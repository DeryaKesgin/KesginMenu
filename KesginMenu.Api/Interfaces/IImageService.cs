using Microsoft.AspNetCore.Http;

namespace KesginMenu.Api.Interfaces;

public interface IImageService
{
    Task<string> UploadImageAsync(IFormFile file);
}