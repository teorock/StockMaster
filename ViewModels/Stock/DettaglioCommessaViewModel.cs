namespace StockMaster.ViewModels.Stock
{
    public class DettaglioCommessaViewModel
    {
        public string CodiceCommessa { get; set; } = string.Empty;
        public string? NomeCliente { get; set; }
        public int? ClienteId { get; set; }
        
        public List<LottoCommessaDto> Lotti { get; set; } = new();
        
        // Statistiche aggregate
        public int TotaleLotti { get; set; }
        public int TotaleUnita { get; set; }
        public int UnitaConsegnate { get; set; }
        public decimal PercentualeCompletamento { get; set; }
        
        public DateTime? DataPrimoIngresso { get; set; }
        public DateTime? DataUltimoMovimento { get; set; }
    }

    public class LottoCommessaDto
    {
        public string LottoRiferimento { get; set; } = string.Empty;
        public string CodiceArticolo { get; set; } = string.Empty;
        public string DescrizioneArticolo { get; set; } = string.Empty;
        public string Fase { get; set; } = string.Empty;
        public int Quantita { get; set; }
        public DateTime DataCreazione { get; set; }
        public string? Note { get; set; }
    }
}