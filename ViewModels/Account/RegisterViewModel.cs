using System.ComponentModel.DataAnnotations;

namespace StockMaster.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username obbligatorio")]
        [StringLength(50, ErrorMessage = "Username max 50 caratteri")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email obbligatoria")]
        [EmailAddress(ErrorMessage = "Email non valida")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password obbligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password min 6 caratteri")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Conferma password obbligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        [Compare("Password", ErrorMessage = "Le password non coincidono")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Nome Completo")]
        public string? NomeCompleto { get; set; }

        [StringLength(50)]
        [Display(Name = "Reparto")]
        public string? Reparto { get; set; }
    }
}