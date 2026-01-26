using System.ComponentModel.DataAnnotations;

namespace StockMaster.Models.Stock
{
    /// <summary>
    /// Anagrafica materiali (es: Rovere, Tiglio, MDF, etc.)
    /// </summary>
    public class Materiale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descrizione { get; set; }

        public bool Attivo { get; set; } = true;

        public DateTime DataCreazione { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<Articolo> Articoli { get; set; } = new List<Articolo>();
    }
}