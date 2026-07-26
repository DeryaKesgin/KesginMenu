using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using KesginMenu.Api.Interfaces;

namespace KesginMenu.Api.Services;

public class ImageService : IImageService
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private readonly Cloudinary _cloudinary;

    public ImageService(IConfiguration configuration)
    {
        var cloudName =
            configuration["Cloudinary:CloudName"];

        var apiKey =
            configuration["Cloudinary:ApiKey"];

        var apiSecret =
            configuration["Cloudinary:ApiSecret"];

        if (string.IsNullOrWhiteSpace(cloudName) ||
            string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(apiSecret) ||
            apiSecret == "BURAYA_API_SECRET")
        {
            throw new InvalidOperationException(
                "Cloudinary bilgileri eksik veya hatalı.");
        }

        var account = new Account(
            cloudName,
            apiKey,
            apiSecret);

        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadImageAsync(
        IFormFile file)
    {
        if (file.Length > MaxFileSize)
        {
            throw new InvalidOperationException(
                "Görsel en fazla 5 MB olabilir.");
        }

        var allowedContentTypes = new[]
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        if (!allowedContentTypes.Contains(file.ContentType))
        {
            throw new InvalidOperationException(
                "Yalnızca JPG, PNG veya WEBP yüklenebilir.");
        }

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(
                file.FileName,
                stream),

            Folder = "kesginmenu/products",

            Transformation = new Transformation()
                .Width(1200)
                .Height(1200)
                .Crop("limit")
                .Quality("auto")
        };

        var result =
            await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
        {
            throw new InvalidOperationException(
                result.Error.Message);
        }

        var imageUrl = result.SecureUrl?.ToString();

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new InvalidOperationException(
                "Cloudinary görsel adresi döndürmedi.");
        }

        return imageUrl;
    }
}