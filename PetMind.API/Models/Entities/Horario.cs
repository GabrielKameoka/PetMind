using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PetMind.API.Models.Entities;

public class Horario
{
    public int Id { get; set; }

    [Required] public int CachorroId { get; set; }

    [Required] public int PetShopId { get; set; }

    [Required] public DateTime Data { get; set; }

    public decimal ValorTotal { get; set; }

    [Required] public string ServicoBaseSelecionado { get; set; } = string.Empty;

    public List<string> Adicionais { get; set; } = new List<string>();

    //UM horário tem UM cachorro
    [JsonIgnore] public Cachorro Cachorro { get; set; } = null;

    [JsonIgnore] public PetShop PetShop { get; set; } = null;
}