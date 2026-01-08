using System.ComponentModel.DataAnnotations;

namespace PetMind.API.Models.DTOs.Horarios;

public class CreateHorarioDto
{
    [Required] public int CachorroId { get; set; }

    [Required] public required string Data { get; set; }

    [Required] public required string ServicoBaseSelecionado { get; set; }

    public List<string> Adicionais { get; set; } = new();
}