using System.ComponentModel.DataAnnotations;

namespace PetMind.API.Models.DTOs.Cachorros
{
    public class UpdateCachorroDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
        public string NomeCachorro { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome do tutor deve ter entre 2 e 100 caracteres")]
        public string NomeTutor { get; set; }

        [Phone(ErrorMessage = "Formato de telefone inválido")]
        public string ContatoTutor { get; set; }

        [StringLength(200, ErrorMessage = "Endereço não pode exceder 200 caracteres")]
        public string EnderecoCachorro { get; set; }

        public string Raca { get; set; }

        [RegularExpression("^(Pequeno|Médio|Grande)$", ErrorMessage = "Porte deve ser Pequeno, Médio ou Grande")]
        public string Porte { get; set; }
    }
}