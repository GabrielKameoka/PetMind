namespace PetMind.API.Models.DTOs.PetShops
{
    public class PetShopResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string EnderecoPetShop { get; set; } = string.Empty;
    }
}