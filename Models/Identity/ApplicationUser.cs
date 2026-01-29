using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StockMaster.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string? NomeCompleto { get; set; }

        [StringLength(50)]
        public string? Reparto { get; set; }

        public bool IsAttivo { get; set; } = true;

        public DateTime DataCreazione { get; set; } = DateTime.Now;

        public DateTime? UltimoAccesso { get; set; }
    }
}