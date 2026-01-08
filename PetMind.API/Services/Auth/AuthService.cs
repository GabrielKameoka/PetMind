using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PetMind.API.Data;
using PetMind.API.Models.DTOs.Auth;
using PetMind.API.Models.DTOs.PetShops;
using PetMind.API.Models.Entities;

namespace PetMind.API.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthService(AppDbContext context, IConfiguration configuration, IMapper mapper)
    {
        _context = context;
        _configuration = configuration;
        _mapper = mapper;
    }

    
    public async Task<AuthResponseDto?> AuthenticateAsync(string email, string senha)
    {
        var petShop = await _context.PetShops
            .FirstOrDefaultAsync(p => p.Email == email);

        if (petShop == null || !BCrypt.Net.BCrypt.Verify(senha, petShop.Senha))
            return null;

        var accessToken = GenerateAccessToken(petShop);
        var (refreshToken, refreshTokenExpiry) = GenerateRefreshToken();

        petShop.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
        petShop.RefreshTokenExpiryTime = refreshTokenExpiry;

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            TokenExpiration = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<int>("JwtSettings:AccessTokenExpirationMinutes")),
            RefreshTokenExpiration = refreshTokenExpiry,
            PetShop = _mapper.Map<PetShopBasicDto>(petShop)
        };
    }


    public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        // Busca apenas tokens válidos
        var petShops = await _context.PetShops
            .Where(p =>
                p.RefreshTokenHash != null &&
                p.RefreshTokenExpiryTime > DateTime.UtcNow)
            .ToListAsync();

        var petShop = petShops.FirstOrDefault(p =>
            BCrypt.Net.BCrypt.Verify(refreshToken, p.RefreshTokenHash!)
        );

        if (petShop == null)
            return null;

        var newAccessToken = GenerateAccessToken(petShop);
        var (newRefreshToken, newRefreshTokenExpiry) = GenerateRefreshToken();

        petShop.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
        petShop.RefreshTokenExpiryTime = newRefreshTokenExpiry;

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            TokenExpiration = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<int>("JwtSettings:AccessTokenExpirationMinutes")),
            RefreshTokenExpiration = newRefreshTokenExpiry,
            PetShop = _mapper.Map<PetShopBasicDto>(petShop)
        };
    }
    
    
    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var petShops = await _context.PetShops
            .Where(p => p.RefreshTokenHash != null)
            .ToListAsync();

        var petShop = petShops.FirstOrDefault(p =>
            BCrypt.Net.BCrypt.Verify(refreshToken, p.RefreshTokenHash!)
        );

        if (petShop == null)
            return false;

        petShop.RefreshTokenHash = null;
        petShop.RefreshTokenExpiryTime = null;

        await _context.SaveChangesAsync();
        return true;
    }

    
    public string GenerateAccessToken(PetShop petShop)
    {
        var secret = _configuration["JwtSettings:Secret"]
                     ?? throw new InvalidOperationException("JWT Secret não configurado");

        var key = Encoding.ASCII.GetBytes(secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, petShop.Id.ToString()),
                new Claim(ClaimTypes.Email, petShop.Email),
                new Claim("PetShopId", petShop.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<int>("JwtSettings:AccessTokenExpirationMinutes")),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    
    public (string refreshToken, DateTime expiryTime) GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        // ✅ Codifica para Base64URL (seguro para URLs/JSON)
        var refreshToken = Base64UrlEncode(randomNumber);

        var expiryTime = DateTime.UtcNow.AddDays(
            _configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays"));

        return (refreshToken, expiryTime);
    }


    // Método auxiliar para Base64URL
    private string Base64UrlEncode(byte[] input)
    {
        // Converte para Base64 padrão
        var base64 = Convert.ToBase64String(input);

        // Substitui caracteres problemáticos
        return base64
            .Replace('+', '-') // Substitui + por -
            .Replace('/', '_') // Substitui / por _
            .Replace("=", ""); // Remove padding =
    }

    private byte[] Base64UrlDecode(string input)
    {
        // Reverte as substituições
        var base64 = input
            .Replace('-', '+')
            .Replace('_', '/');

        // Adiciona padding se necessário
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}