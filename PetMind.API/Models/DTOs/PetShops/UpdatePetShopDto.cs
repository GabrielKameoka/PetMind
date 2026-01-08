using System.ComponentModel.DataAnnotations;

namespace PetMind.API.Models.DTOs.PetShops
{
    public class UpdatePetShopDto
    {
        [StringLength(200, ErrorMessage = "Endereço não pode exceder 200 caracteres")]
        public string EnderecoPetShop { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        public string NovaSenha { get; set; }

        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem")]
        public string ConfirmarNovaSenha { get; set; }
        
        [RequiredWhenChangingPassword]
        public string? SenhaAtual { get; set; }
    }

    // Atributo customizado para validar quando está alterando senha
    public class RequiredWhenChangingPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dto = (UpdatePetShopDto)validationContext.ObjectInstance;
        
            // Se está tentando alterar a senha, a senha atual é obrigatória
            if (!string.IsNullOrEmpty(dto.NovaSenha) && string.IsNullOrEmpty(value?.ToString()))
            {
                return new ValidationResult("Senha atual é obrigatória para alterar a senha");
            }
        
            return ValidationResult.Success;
        }
    }
}