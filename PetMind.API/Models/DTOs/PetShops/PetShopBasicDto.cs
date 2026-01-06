namespace PetMind.API.Models.DTOs.PetShops;

public class PetShopBasicDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string EnderecoPetShop { get; set; } = string.Empty;
}