using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using PetMind.API.Services.Auth;

namespace PetMind.API.Middleware;

public class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;

    public TokenRefreshMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        var endpoint = context.GetEndpoint();
        var hasAuthorize = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>() != null;

        if (hasAuthorize && !context.User.Identity.IsAuthenticated)
        {
            var refreshToken = context.Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var authResponse = await authService.RefreshTokenAsync(refreshToken);
                if (authResponse != null)
                {
                    // Adiciona o novo token ao header da requisição atual
                    context.Request.Headers["Authorization"] = $"Bearer {authResponse.Token}";
                }
            }
        }

        await _next(context);
    }
}