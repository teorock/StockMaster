using System.ComponentModel.DataAnnotations;

namespace StockMaster.Models.Stock
{
    /// <summary>
    /// Definizione fasi di lavorazione possibili
    /// </summary>
    public class FaseLavorazione: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Codice { get; set; } = string.Empty; // es: "grezzo_stock", "lav_nesting"

        [Required]
        [StringLength(100)]
        public string Descrizione { get; set; } = string.Empty; // es: "Grezzo in Stock", "Lavorazione Nesting"

        public int OrdineSequenza { get; set; } // Per ordinare le fasi nel workflow

        public bool Attivo { get; set; } = true;

        public string? Note { get; set; }
    }
}