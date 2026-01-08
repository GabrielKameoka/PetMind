using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetMind.API.Data;
using PetMind.API.Services;
using Scalar.AspNetCore;
using System.Text;
using PetMind.API.Middleware;
using PetMind.API.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

// Configurar serviços
builder.Services.AddControllers();

// Configurar OpenAPI (Swagger no .NET 9)
builder.Services.AddOpenApi();

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Configurar banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"] 
    ?? throw new InvalidOperationException("JWT Secret não configurado no appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secret)),
        ClockSkew = TimeSpan.FromMinutes(1), // Um pouco de tolerância
        NameClaimType = ClaimTypes.NameIdentifier, // IMPORTANTE!
        RoleClaimType = ClaimTypes.Role
    };
    
    // Para .NET 8+, isso ajuda com claims
    options.MapInboundClaims = false;
    
    // Eventos para debug MELHORADOS
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var principal = context.Principal;
            Console.WriteLine("=== JWT VALIDATION SUCCESS ===");
            Console.WriteLine($"User authenticated: {principal?.Identity?.IsAuthenticated}");
            Console.WriteLine($"User name: {principal?.Identity?.Name}");
            
            if (principal != null)
            {
                foreach (var claim in principal.Claims)
                {
                    Console.WriteLine($"  Claim: {claim.Type} = {claim.Value}");
                }
                
                // Verifica claims específicas
                var nameId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                var petShopId = principal.FindFirst("PetShopId")?.Value;
                
                Console.WriteLine($"NameIdentifier: {nameId}");
                Console.WriteLine($"Email: {email}");
                Console.WriteLine($"PetShopId: {petShopId}");
            }
            
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("=== JWT VALIDATION FAILED ===");
            Console.WriteLine($"Exception: {context.Exception.Message}");
            Console.WriteLine($"Exception type: {context.Exception.GetType().Name}");
            
            if (context.Exception is SecurityTokenExpiredException)
            {
                Console.WriteLine("Token EXPIRED!");
            }
            else if (context.Exception is SecurityTokenInvalidSignatureException)
            {
                Console.WriteLine("Invalid signature!");
            }
            
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"=== JWT CHALLENGE ===");
            Console.WriteLine($"Error: {context.Error}");
            Console.WriteLine($"ErrorDescription: {context.ErrorDescription}");
            Console.WriteLine($"Request Path: {context.Request.Path}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Registrar serviços
builder.Services.AddScoped<IValidaRacaService, ValidaRacaService>();
builder.Services.AddScoped<CalculaPrecosService>();
builder.Services.AddScoped<IAuthService, AuthService>(); // Adicionar serviço de autenticação

// No método ConfigureServices (no Program.cs)
builder.Services.AddHttpContextAccessor();

// Configurar CORS (se precisar de frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "http://localhost:3000") // Angular/React
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    
    // Configurar documentação OpenAPI/Swagger no .NET 9
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "PetMind API V1");
        options.RoutePrefix = "swagger"; // Acessar em /swagger
        
        // Configurar autenticação JWT no Swagger UI
        options.OAuthClientId("swagger-ui");
        options.OAuthAppName("PetMind API - Swagger");
        options.OAuthUsePkce();
    });
    
    // Ativar CORS apenas em desenvolvimento
    app.UseCors("AllowFrontend");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TokenRefreshMiddleware>();

app.MapControllers();

app.Run();