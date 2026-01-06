using System.ComponentModel.DataAnnotations;

namespace PetMind.API.Models.DTOs.Horarios
{
    public class HorarioResponseDto
    {
        public int Id { get; set; }
        public string Data { get; set; } = string.Empty;
        public string ServicoBaseSelecionado { get; set; } = string.Empty;
        public List<string> Adicionais { get; set; } = new();

        [Required] public decimal ValorTotal { get; set; }

        public CachorroInfoDto? Cachorro { get; set; }
        public PetShopInfoDto? PetShop { get; set; }
    }

    public class CachorroInfoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Raca { get; set; } = string.Empty;
        public string Porte { get; set; } = string.Empty;
        public string NomeTutor { get; set; } = string.Empty;
        public string ContatoTutor { get; set; } = string.Empty;
    }

    public class PetShopInfoDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string EnderecoPetShop { get; set; } = string.Empty;
    }
}