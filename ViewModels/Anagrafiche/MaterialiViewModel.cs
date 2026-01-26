using StockMaster.Models.Stock;

namespace StockMaster.ViewModels.Anagrafiche
{
    public class MaterialiViewModel
    {
        public List<Materiale> Materiali { get; set; } = new();
        public Materiale? MaterialeCorrente { get; set; }
        public bool IsEdit { get; set; }
    }
}