using PetMind.API.Models.Entities;

namespace PetMind.API.Models.DTOs.Cachorros
{
    public class CachorroResponseDto
    {
        public int Id { get; set; }
        public string NomeCachorro { get; set; } = string.Empty;
        public string NomeTutor { get; set; } = string.Empty;
        public string ContatoTutor { get; set; } = string.Empty;
        public string EnderecoCachorro { get; set; } = string.Empty;
        public string Raca { get; set; } = string.Empty;
        public string Porte { get; set; } = string.Empty;
        
        public int PetShopId { get; set; }
        
    }
}