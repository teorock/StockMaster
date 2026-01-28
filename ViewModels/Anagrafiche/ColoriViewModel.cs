using StockMaster.Models.Stock;

namespace StockMaster.ViewModels.Anagrafiche
{
    public class ColoriViewModel
    {
        public List<Colore> Colori { get; set; } = new();
        public Colore? ColoreCorrente { get; set; }
        public bool IsEdit { get; set; }
    }
}