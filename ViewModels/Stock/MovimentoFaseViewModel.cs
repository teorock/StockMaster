using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StockMaster.ViewModels.Stock
{
    public class MovimentoFaseViewModel
    {
        [Required(ErrorMessage = "Seleziona un lotto")]
        [Display(Name = "Lotto")]
        public string LottoRiferimento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleziona la fase di partenza")]
        [Display(Name = "Da Fase")]
        public string FaseDa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleziona la fase di arrivo")]
        [Display(Name = "A Fase")]
        public string FaseA { get; set; } = string.Empty;

        [Required(ErrorMessage = "Inserisci la quantità")]
        [Range(0, int.MaxValue, ErrorMessage = "La quantità deve essere maggiore o uguale a 0")]
        [Display(Name = "Quantità (da spostare)")]
        public int Quantita { get; set; }

        // NUOVO: Gestione scarti
        [Range(0, int.MaxValue, ErrorMessage = "La quantità scartata deve essere maggiore o uguale a 0")]
        [Display(Name = "Quantità Scartata (opzionale)")]
        public int QuantitaScartata { get; set; } = 0;

        [Display(Name = "Motivo Scarto")]
        [DataType(DataType.MultilineText)]
        public string? MotivoScarto { get; set; }

        [Display(Name = "Cliente (opzionale)")]
        public int? ClienteId { get; set; }

        [Display(Name = "Codice Commessa (opzionale)")]
        public string? CodiceCommessa { get; set; }

        [Display(Name = "Note")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        // Info del lotto selezionato (per mostrare dettagli)
        public string? CodiceArticolo { get; set; }
        public string? DescrizioneArticolo { get; set; }
        public int QuantitaDisponibile { get; set; }

        // Dropdown lists
        public List<SelectListItem> Lotti { get; set; } = new();
        public List<SelectListItem> Fasi { get; set; } = new();
        public List<SelectListItem> Clienti { get; set; } = new();
    }
}