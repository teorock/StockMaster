using Microsoft.AspNetCore.Mvc;
using StockMaster.Services.Stock;

namespace StockMaster.Controllers
{
    public class TestMagazzinoController : Controller
    {
        private readonly MagazzinoService _magazzinoService;

        public TestMagazzinoController(MagazzinoService magazzinoService)
        {
            _magazzinoService = magazzinoService;
        }

        // GET: /TestMagazzino/TestIngresso
        public async Task<IActionResult> TestIngresso()
        {
            try
            {
                // Simula ingresso 100 pannelli
                var stock = await _magazzinoService.IngressoMerceAsync(
                    articoloId: 1, // Assicurati che esista nel DB!
                    codiceArticolo: "PAN-ROV-2440-18-MEL",
                    quantita: 100,
                    fornitoreId: 1,
                    note: "Test ingresso automatico"
                );

                return Content($"✅ Ingresso OK! Lotto: {stock.LottoRiferimento}, Quantità: {stock.Quantita}");
            }
            catch (Exception ex)
            {
                return Content($"❌ Errore: {ex.Message}");
            }
        }

        // GET: /TestMagazzino/TestMovimento
        public async Task<IActionResult> TestMovimento(string lotto)
        {
            try
            {
                var result = await _magazzinoService.SpostaFaseAsync(
                    lottoRiferimento: lotto,
                    faseDa: "grezzo_stock",
                    faseA: "lav_nesting",
                    quantita: 30,
                    note: "Test movimento automatico"
                );

                return Content($"✅ Movimento OK! Spostati 30 pz da grezzo a nesting");
            }
            catch (Exception ex)
            {
                return Content($"❌ Errore: {ex.Message}");
            }
        }

        // GET: /TestMagazzino/Stato
        public async Task<IActionResult> Stato()
        {
            var stato = await _magazzinoService.GetStatoMagazzinoAsync();

            var html = "<h2>Stato Magazzino</h2><table border='1'><tr><th>Fase</th><th>Unità</th><th>Lotti</th></tr>";
            foreach (var s in stato)
            {
                html += $"<tr><td>{s.Fase}</td><td>{s.TotaleUnita}</td><td>{s.NumeroLotti}</td></tr>";
            }
            html += "</table>";

            return Content(html, "text/html");
        }
    }
}