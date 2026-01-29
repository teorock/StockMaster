using System.ComponentModel.DataAnnotations;

namespace StockMaster.Models.Stock
{
    /// <summary>
    /// Anagrafica fornitori
    /// </summary>
    public class Fornitore: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(100)]
        public string? PartitaIva { get; set; }

        [StringLength(200)]
        public string? Indirizzo { get; set; }

        [StringLength(50)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public string? Note { get; set; }

        public bool Attivo { get; set; } = true;

        public DateTime DataCreazione { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<ArticoloStock> Forniture { get; set; } = new List<ArticoloStock>();
    }
}