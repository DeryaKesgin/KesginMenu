using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KesginMenu.Api.DTOs;
using KesginMenu.Api.Entities;
using KesginMenu.Api.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace KesginMenu.Api.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoginResponseDto CreateToken(AppUser user)
    {
        var keyValue = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT anahtarı bulunamadı.");

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var expirationMinutes =
            int.TryParse(
                _configuration["Jwt:ExpirationMinutes"],
                out var minutes)
                ? minutes
                : 120;

        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("businessId", user.BusinessId.ToString())
        };

        var securityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            UserId = user.Id,
            BusinessId = user.BusinessId,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };
    }
}