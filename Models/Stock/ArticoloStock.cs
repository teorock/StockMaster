using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMaster.Models.Stock
{
    /// <summary>
    /// Rappresenta una quantità di articoli in una specifica fase di lavorazione
    /// Ogni record = lotto omogeneo per articolo + fase
    /// </summary>
    public class ArticoloStock: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        // Riferimento articolo tramite Id (foreign key standard)
        public int ArticoloId { get; set; }

        public virtual Articolo? Articolo { get; set; }

        // Codice articolo denormalizzato (per comodità query/display)
        [Required]
        [StringLength(50)]
        public string CodiceArticolo { get; set; } = string.Empty;

        // Fase corrente
        [Required]
        [StringLength(50)]
        public string Fase { get; set; } = string.Empty;

        // Quantità in questa fase
        [Required]
        public int Quantita { get; set; }

        // Tracciabilità lotto
        [Required]
        [StringLength(100)]
        public string LottoRiferimento { get; set; } = string.Empty;

        // Riferimenti esterni
        public int? FornitoreId { get; set; }
        [ForeignKey(nameof(FornitoreId))]
        public virtual Fornitore? Fornitore { get; set; }

        public int? ClienteId { get; set; }
        [ForeignKey(nameof(ClienteId))]
        public virtual Cliente? Cliente { get; set; }

        // Campo per integrazione futura con PFC
        [StringLength(50)]
        public string? CodiceCommessa { get; set; }

        // Metadati
        public DateTime DataCreazione { get; set; } = DateTime.Now;

        public string? Note { get; set; }

        // Codice display generato (per etichette)
        [NotMapped]
        public string CodiceDisplay => $"[{GetFaseSigla()}] {LottoRiferimento}";

        private string GetFaseSigla()
        {
            return Fase switch
            {
                "grezzo_stock" => "GRZ",
                "lav_nesting" => "NES",
                "lav_bordatura" => "BOR",
                "verniciatura" => "VER",
                "rilavorazione" => "RIL",
                "finito_stock" => "FIN",
                "consegnato" => "CON",
                _ => "???"
            };
        }
    }
}