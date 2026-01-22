using StockMaster.Models.Stock;

namespace StockMaster.ViewModels.Anagrafiche
{
    public class FornitoriViewModel
    {
        public List<Fornitore> Fornitori { get; set; } = new();
        public Fornitore? FornitoreCorrente { get; set; }
        public bool IsEdit { get; set; }
    }
}