using System.ComponentModel.DataAnnotations;

namespace PetMind.API.Models.DTOs.Horarios;

public class CreateHorarioDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ID do cachorro inválido")]
    public int CachorroId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ID do petshop inválido")]
    public int PetShopId { get; set; }

    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Data { get; set; }

    [Required(ErrorMessage = "Serviço base é obrigatório")]
    public string ServicoBaseSelecionado { get; set; } = string.Empty;

    public List<string> Adicionais { get; set; } = new();
}