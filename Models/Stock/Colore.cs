using System.ComponentModel.DataAnnotations;

namespace StockMaster.Models.Stock
{
    /// <summary>
    /// Anagrafica colori (es: Bianco, Nero, Rovere Naturale, etc.)
    /// </summary>
    public class Colore
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descrizione { get; set; }

        [StringLength(7)]
        public string? CodiceHex { get; set; } // es: "#FFFFFF" per preview visiva

        public bool Attivo { get; set; } = true;

        public DateTime DataCreazione { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<Articolo> Articoli { get; set; } = new List<Articolo>();
    }
}