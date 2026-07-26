using KesginMenu.Api.DTOs;
using KesginMenu.Api.Entities;
using KesginMenu.Api.Exceptions;
using KesginMenu.Api.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace KesginMenu.Api.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<AppUser> _passwordHasher;

    public AuthService(
        IAuthRepository authRepository,
        ITokenService tokenService,
        PasswordHasher<AppUser> passwordHasher)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new BusinessRuleException(
                "E-posta ve şifre zorunludur.");
        }

        var user = await _authRepository.GetByEmailAsync(email);

        if (user is null || !user.IsActive)
        {
            throw new BusinessRuleException(
                "E-posta veya şifre hatalı.",
                401);
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new BusinessRuleException(
                "E-posta veya şifre hatalı.",
                401);
        }

        return _tokenService.CreateToken(user);
    }
}