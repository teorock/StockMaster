using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StockMaster.ViewModels.Stock
{
    public class IngressoMerceViewModel
    {
        [Required(ErrorMessage = "Seleziona un articolo")]
        [Display(Name = "Articolo")]
        public int ArticoloId { get; set; }

        [Required(ErrorMessage = "Seleziona un fornitore")]
        [Display(Name = "Fornitore")]
        public int FornitoreId { get; set; }

        [Required(ErrorMessage = "Inserisci la quantità")]
        [Range(1, int.MaxValue, ErrorMessage = "La quantità deve essere maggiore di 0")]
        [Display(Name = "Quantità")]
        public int Quantita { get; set; }

        [Display(Name = "Codice Commessa (opzionale)")]
        public string? CodiceCommessa { get; set; }

        [Display(Name = "Note")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        // Dropdown lists (popolate dal controller)
        public List<SelectListItem> Articoli { get; set; } = new();
        public List<SelectListItem> Fornitori { get; set; } = new();
    }
}