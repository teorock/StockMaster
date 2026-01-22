using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockMaster.ViewModels.Stock
{
    public class DashboardViewModel
    {
        public List<StatoFaseCard> StatoPerFase { get; set; } = new();
        public List<UltimoMovimentoDto> UltimiMovimenti { get; set; } = new();
        public int TotaleArticoliGestiti { get; set; }
        public int TotaleLottiAttivi { get; set; }

        // NUOVO: Filtro cliente
        public int? ClienteSelezionatoId { get; set; }
        public List<SelectListItem> Clienti { get; set; } = new();        
    }

    public class StatoFaseCard
    {
        public string Fase { get; set; } = string.Empty;
        public string FaseDescrizione { get; set; } = string.Empty;
        public int TotaleUnita { get; set; }
        public int NumeroLotti { get; set; }
        public string CssClass { get; set; } = "primary"; // per colori cards
    }

    public class UltimoMovimentoDto
    {
        public string LottoRiferimento { get; set; } = string.Empty;
        public string CodiceArticolo { get; set; } = string.Empty;
        public string? FaseDa { get; set; }
        public string FaseA { get; set; } = string.Empty;
        public int Quantita { get; set; }
        public DateTime DataTransizione { get; set; }
        public string? Note { get; set; }
    }
}