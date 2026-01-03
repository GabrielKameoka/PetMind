using System.ComponentModel.DataAnnotations;
using PetMind.API.Models.Entities;

namespace PetMind.API.Models.DTOs.PetShops
{
    public class CreatePetShopDto
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        public string Senha { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Endereço não pode exceder 200 caracteres")]
        public string EnderecoPetShop { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("Senha", ErrorMessage = "As senhas não conferem")]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }
}