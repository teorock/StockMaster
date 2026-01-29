using System.ComponentModel.DataAnnotations;

namespace StockMaster.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username o Email obbligatorio")]
        [Display(Name = "Username o Email")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password obbligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ricordami")]
        public bool RememberMe { get; set; }
    }
}