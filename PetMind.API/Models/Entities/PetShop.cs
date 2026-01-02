using System.ComponentModel.DataAnnotations;
namespace PetMind.API.Models.Entities;

public class PetShop
{
    public int Id { get; set; }
    
    [Required]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Senha { get; set; } = string.Empty;
    
    public string EnderecoPetShop { get; set; } = string.Empty;
    
    public List<Horario> Horarios { get; set; } = new List<Horario>();
}