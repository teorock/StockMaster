using System.ComponentModel.DataAnnotations;

namespace StockMaster.Models.Stock
{
    /// <summary>
    /// Storico di tutte le transizioni di fase
    /// Permette tracciabilità completa e analisi temporali
    /// </summary>
    public class TransizioneFase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string LottoRiferimento { get; set; } = string.Empty;

        // Fase di partenza (NULL se è l'ingresso iniziale)
        [StringLength(50)]
        public string? FaseDa { get; set; }

        // Fase di arrivo
        [Required]
        [StringLength(50)]
        public string FaseA { get; set; } = string.Empty;

        [Required]
        public int Quantita { get; set; }

        [Required]
        public DateTime DataTransizione { get; set; } = DateTime.Now;

        // Chi ha effettuato la transizione
        [StringLength(100)]
        public string? OperatoreId { get; set; } // Riferimento all'utente Identity

        public string? Note { get; set; }
    }
}