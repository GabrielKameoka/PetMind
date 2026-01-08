using PetMind.API.Models.DTOs.Auth;
using PetMind.API.Models.Entities;

namespace PetMind.API.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto?> AuthenticateAsync(string email, string senha);
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
    string GenerateAccessToken(PetShop petShop);
    (string refreshToken, DateTime expiryTime) GenerateRefreshToken();
}