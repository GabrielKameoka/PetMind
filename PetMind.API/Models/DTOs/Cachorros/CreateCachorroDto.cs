using System.ComponentModel.DataAnnotations;

namespace PetMind.API.Models.DTOs.Cachorros
{
    public class CreateCachorroDto
    {
        [Required(ErrorMessage = "Nome do cachorro é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
        public string NomeCachorro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome do tutor é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome do tutor deve ter entre 2 e 100 caracteres")]
        public string NomeTutor { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contato do tutor é obrigatório")]
        [Phone(ErrorMessage = "Formato de telefone inválido")]
        public string ContatoTutor { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Endereço não pode exceder 200 caracteres")]
        public string EnderecoCachorro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Raça é obrigatória")]
        public string Raca { get; set; } = string.Empty;

        [Required(ErrorMessage = "Porte é obrigatório")]
        [RegularExpression("^(Pequeno|Médio|Grande)$", ErrorMessage = "Porte deve ser Pequeno, Médio ou Grande")]
        public string Porte { get; set; } = string.Empty;
    }
}