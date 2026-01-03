using System.ComponentModel.DataAnnotations;

namespace PetMind.API.Models.DTOs.Horarios
{
    public class UpdateHorarioDto
    {
        public DateTime? Data { get; set; }

        public string? ServicoBaseSelecionado { get; set; }

        public List<string>? Adicionais { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ID do cachorro inválido")]
        public int? CachorroId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ID do petshop inválido")]
        public int? PetShopId { get; set; }
    }
}