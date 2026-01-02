namespace PetMind.API.Models;

public class ServicoBase
{
    public string Porte { get; set; } = string.Empty;
    public string Raca { get; set; } = string.Empty;
    public Dictionary<string, decimal> Servicos { get; set; } = new();
}