using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetMind.API.Data;
using Microsoft.EntityFrameworkCore;
using PetMind.API.Models.DTOs.Auth;
using PetMind.API.Services.Auth;

namespace PetMind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthController(IAuthService authService, IConfiguration configuration, AppDbContext context)
    {
        _authService = authService;
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var authResponse = await _authService.AuthenticateAsync(
            loginDto.Email,
            loginDto.Senha);

        if (authResponse == null)
            return Unauthorized(new { Message = "Email ou senha incorretos" });

        // Coloca o refresh token em um cookie seguro (opcional)
        SetRefreshTokenCookie(authResponse.RefreshToken);

        return Ok(authResponse);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshTokenDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Pode pegar do corpo OU do cookie
        var refreshToken = dto.RefreshToken
                           ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(new { Message = "Refresh token é obrigatório" });

        var authResponse = await _authService.RefreshTokenAsync(refreshToken);

        if (authResponse == null)
            return Unauthorized(new { Message = "Refresh token inválido ou expirado" });

        // Atualiza o cookie
        SetRefreshTokenCookie(authResponse.RefreshToken);

        return Ok(authResponse);
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke(RefreshTokenDto dto)
    {
        var result = await _authService.RevokeTokenAsync(dto.RefreshToken);

        if (!result)
            return BadRequest(new { Message = "Refresh token inválido" });

        // Remove o cookie
        Response.Cookies.Delete("refreshToken");

        return Ok(new { Message = "Token revogado com sucesso" });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var petShopId = User.FindFirst("PetShopId")?.Value;

        if (string.IsNullOrEmpty(petShopId))
        {
            // Se não tem PetShopId no token, tenta pelo refresh token do cookie
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authService.RevokeTokenAsync(refreshToken);
            }
        }
        else if (int.TryParse(petShopId, out int id))
        {
            // Limpa refresh token no banco
            var petShop = await _context.PetShops.FindAsync(id);
            if (petShop != null)
            {
                petShop.RefreshTokenHash = null;
                petShop.RefreshTokenExpiryTime = null;
                await _context.SaveChangesAsync();
            }
        }

        // Remove cookie
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new { Message = "Logout realizado com sucesso" });
    }

    [HttpGet("validate")]
    [Authorize]
    public IActionResult Validate()
    {
        var petShopId = User.FindFirst("PetShopId")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new
        {
            Message = "Token válido",
            PetShopId = petShopId,
            Email = email
        });
    }

    [HttpGet("debug")]
    [Authorize]
    public IActionResult DebugClaims()
    {
        var claims = User.Claims.Select(c => new
        {
            Type = c.Type,
            Value = c.Value,
            ShortType = c.Type.Split('/').Last()
        }).ToList();

        return Ok(new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated,
            Name = User.Identity?.Name,
            AllClaims = claims,
            NameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value,
            PetShopId = User.FindFirst("PetShopId")?.Value
        });
    }

    // Método auxiliar para cookies
    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Não acessível via JavaScript
            Secure = true, // Apenas HTTPS (em produção)
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(
                _configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays"))
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}