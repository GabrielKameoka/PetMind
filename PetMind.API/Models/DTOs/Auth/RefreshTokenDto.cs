using System.ComponentModel.DataAnnotations;

namespace PetMind.API.Models.DTOs.Auth;

public class RefreshTokenDto
{
    [Required(ErrorMessage = "Refresh Token é obrigatório")]
    public string RefreshToken { get; set; } = string.Empty;
}