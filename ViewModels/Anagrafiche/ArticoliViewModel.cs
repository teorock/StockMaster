using StockMaster.Models.Stock;

namespace StockMaster.ViewModels.Anagrafiche
{
    public class ArticoliViewModel
    {
        public List<Articolo> Articoli { get; set; } = new();
        public Articolo? ArticoloCorrente { get; set; }
        public bool IsEdit { get; set; }
    }

    public class ImportCsvResult
    {
        public bool Success { get; set; }
        public int RecordImportati { get; set; }
        public int RecordScartati { get; set; }
        public List<string> Errori { get; set; } = new();
        public string? Message { get; set; }
    }
}