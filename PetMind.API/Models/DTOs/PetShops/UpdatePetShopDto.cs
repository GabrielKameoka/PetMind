using System.ComponentModel.DataAnnotations;

namespace PetMind.API.Models.DTOs.PetShops
{
    public class UpdatePetShopDto
    {
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "Endereço não pode exceder 200 caracteres")]
        public string? EnderecoPetShop { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        public string? NovaSenha { get; set; }

        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem")]
        public string? ConfirmarNovaSenha { get; set; }
    }
}