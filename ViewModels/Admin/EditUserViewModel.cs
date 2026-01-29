using System.ComponentModel.DataAnnotations;

namespace StockMaster.ViewModels.Admin
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email obbligatoria")]
        [EmailAddress(ErrorMessage = "Email non valida")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Nome Completo")]
        public string? NomeCompleto { get; set; }

        [StringLength(50)]
        [Display(Name = "Reparto")]
        public string? Reparto { get; set; }

        [Display(Name = "Utente Attivo")]
        public bool IsAttivo { get; set; }

        // Gestione ruoli
        public List<string> CurrentRoles { get; set; } = new();
        public List<string> AllRoles { get; set; } = new();
        public List<string>? SelectedRoles { get; set; }
    }
}