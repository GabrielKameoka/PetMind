using System.ComponentModel.DataAnnotations;
namespace PetMind.API.Models.Entities;

public class Cachorro
{
    public int Id { get; set; }
    
    [Required]
    public string NomeCachorro { get; set; } = string.Empty;
    
    [Required]
    public string NomeTutor { get; set; } = string.Empty;
    
    [Required]
    public string ContatoTutor { get; set; } = string.Empty;
    
    public string EnderecoCachorro { get; set; } = string.Empty;
    
    [Required]
    public string Raca { get; set; } = string.Empty;
    
    [Required]
    public string Porte { get; set; } = string.Empty;
}