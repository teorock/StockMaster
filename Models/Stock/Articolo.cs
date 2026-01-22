using System.ComponentModel.DataAnnotations;

namespace StockMaster.Models.Stock
{
    /// <summary>
    /// Anagrafica articoli/prodotti gestiti a magazzino
    /// </summary>
    public class Articolo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Codice { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Descrizione { get; set; } = string.Empty;

        [StringLength(10)]
        public string? UnitaMisura { get; set; } // es: "PZ", "MQ", "ML"

        [StringLength(50)]
        public string? Categoria { get; set; } // es: "Pannelli", "Bordi", "Accessori"

        [StringLength(50)]
        public string? Materiale { get; set; } // es: "Hpl", "Truciolare", "Mdf"        

        public string? Note { get; set; }

        public DateTime DataCreazione { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<ArticoloStock> StockRecords { get; set; } = new List<ArticoloStock>();
    }
}