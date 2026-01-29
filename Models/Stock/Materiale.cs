using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMaster.Models.Stock
{
    /// <summary>
    /// Anagrafica materiali (es: Rovere, Tiglio, MDF, etc.)
    /// </summary>
    public class Materiale: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descrizione { get; set; }

        // NUOVO: Colore del materiale
        public int? ColoreId { get; set; }
        
        [ForeignKey(nameof(ColoreId))]
        public virtual Colore? Colore { get; set; }

        public bool Attivo { get; set; } = true;

        public DateTime DataCreazione { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<Articolo> Articoli { get; set; } = new List<Articolo>();
    }
}