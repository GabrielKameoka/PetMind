namespace PetMind.API.Models.DTOs.Horarios
{
    public class CreateHorarioDto
    {
        public List<int> CachorroIds { get; set; } = new();
        public int PetShopId { get; set; }
        public DateTime Data { get; set; }
        public string ServicoBaseSelecionado { get; set; } = string.Empty;
        public List<string> Adicionais { get; set; } = new();
    }
}