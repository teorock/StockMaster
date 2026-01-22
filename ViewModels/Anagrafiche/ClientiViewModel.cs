using StockMaster.Models.Stock;

namespace StockMaster.ViewModels.Anagrafiche
{
    public class ClientiViewModel
    {
        public List<Cliente> Clienti { get; set; } = new();
        public Cliente? ClienteCorrente { get; set; }
        public bool IsEdit { get; set; }
    }
}