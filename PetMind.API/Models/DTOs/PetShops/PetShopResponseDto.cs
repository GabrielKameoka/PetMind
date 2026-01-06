using PetMind.API.Models.DTOs.Horarios;

namespace PetMind.API.Models.DTOs.PetShops
{
    public class PetShopResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string EnderecoPetShop { get; set; } = string.Empty;
        public List<HorarioResponseDto> Horarios { get; set; } = new List<HorarioResponseDto>();
    }
    
    public class HorarioInfoDto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public decimal ValorTotal { get; set; }
        public string ServicoBaseSelecionado { get; set; }
        public List<string> Adicionais { get; set; } = new List<string>();
        public CachorroInfoDto Cachorro { get; set; }
    }
}