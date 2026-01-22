using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockMaster.Data;
using StockMaster.Models.Stock;
using StockMaster.ViewModels.Anagrafiche;
using System.Globalization;
using System.Text;

namespace StockMaster.Controllers
{
    public class AnagraficheController : Controller
    {
        private readonly StockDbContext _context;

        public AnagraficheController(StockDbContext context)
        {
            _context = context;
        }

        // ========== ARTICOLI ==========

        // GET: /Anagrafiche/Articoli
        public async Task<IActionResult> Articoli()
        {
            var viewModel = new ArticoliViewModel
            {
                Articoli = await _context.Articoli.OrderBy(a => a.Codice).ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvaArticolo(Articolo articolo)
        {
            if (!ModelState.IsValid)
            {
                var errori = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["ErrorMessage"] = "❌ Dati non validi: " + string.Join(", ", errori);
                TempData["ArticoloCorrente"] = System.Text.Json.JsonSerializer.Serialize(articolo);
                TempData["MostraModal"] = "modalNuovoArticolo";
                
                return RedirectToAction(nameof(Articoli));
            }

            try
            {
                if (articolo.Id == 0)
                {
                    articolo.DataCreazione = DateTime.Now;
                    _context.Articoli.Add(articolo);
                }
                else
                {
                    _context.Articoli.Update(articolo);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"✅ Articolo '{articolo.Codice}' salvato";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = $"❌ Errore: {ex.InnerException?.Message ?? ex.Message}";
                TempData["ArticoloCorrente"] = System.Text.Json.JsonSerializer.Serialize(articolo);
                TempData["MostraModal"] = "modalNuovoArticolo";
            }

            return RedirectToAction(nameof(Articoli));
        }

        // POST: /Anagrafiche/EliminaArticolo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminaArticolo(int id)
        {
            try
            {
                var articolo = await _context.Articoli.FindAsync(id);
                if (articolo == null)
                {
                    TempData["ErrorMessage"] = "Articolo non trovato";
                    return RedirectToAction(nameof(Articoli));
                }

                // Verifica che non ci siano movimenti associati
                var haMovimenti = await _context.ArticoliStock.AnyAsync(s => s.ArticoloId == id);
                if (haMovimenti)
                {
                    TempData["ErrorMessage"] = "❌ Impossibile eliminare: articolo presente in magazzino";
                    return RedirectToAction(nameof(Articoli));
                }

                _context.Articoli.Remove(articolo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"✅ Articolo '{articolo.Codice}' eliminato";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"❌ Errore: {ex.Message}";
            }

            return RedirectToAction(nameof(Articoli));
        }

        // POST: /Anagrafiche/ImportArticoliCsv
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportArticoliCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "❌ Nessun file selezionato";
                return RedirectToAction(nameof(Articoli));
            }

            var result = new ImportCsvResult();

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                {
                    // Salta header (prima riga)
                    await reader.ReadLineAsync();

                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        var values = line.Split(';'); // CSV separato da punto e virgola

                        if (values.Length < 2)
                        {
                            result.Errori.Add($"Riga ignorata (pochi campi): {line}");
                            result.RecordScartati++;
                            continue;
                        }

                        try
                        {
                            var codice = values[0].Trim();
                            var descrizione = values[1].Trim();
                            var unitaMisura = values.Length > 2 ? values[2].Trim() : "PZ";
                            var categoria = values.Length > 3 ? values[3].Trim() : null;

                            // Verifica se esiste già
                            var esistente = await _context.Articoli.FirstOrDefaultAsync(a => a.Codice == codice);

                            if (esistente == null)
                            {
                                // Crea nuovo
                                var articolo = new Articolo
                                {
                                    Codice = codice,
                                    Descrizione = descrizione,
                                    UnitaMisura = unitaMisura,
                                    Categoria = categoria,
                                    DataCreazione = DateTime.Now
                                };

                                _context.Articoli.Add(articolo);
                                result.RecordImportati++;
                            }
                            else
                            {
                                // Aggiorna esistente
                                esistente.Descrizione = descrizione;
                                esistente.UnitaMisura = unitaMisura;
                                esistente.Categoria = categoria;

                                result.RecordImportati++;
                            }
                        }
                        catch (Exception ex)
                        {
                            result.Errori.Add($"Errore riga '{line}': {ex.Message}");
                            result.RecordScartati++;
                        }
                    }

                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = $"✅ Import completato: {result.RecordImportati} articoli importati/aggiornati";

                    if (result.RecordScartati > 0)
                    {
                        result.Message += $", {result.RecordScartati} scartati";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"❌ Errore durante l'import: {ex.Message}";
            }

            TempData["SuccessMessage"] = result.Message;
            if (result.Errori.Any())
            {
                TempData["ImportErrors"] = string.Join("<br>", result.Errori.Take(10));
            }

            return RedirectToAction(nameof(Articoli));
        }

        // ========== FORNITORI ==========

        public async Task<IActionResult> Fornitori()
        {
            var viewModel = new FornitoriViewModel
            {
                Fornitori = await _context.Fornitori.OrderBy(f => f.Nome).ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvaFornitore(Fornitore fornitore)
        {
            if (!ModelState.IsValid)
            {
                var errori = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["ErrorMessage"] = "❌ Dati non validi: " + string.Join(", ", errori);
                TempData["FornitoreCorrente"] = System.Text.Json.JsonSerializer.Serialize(fornitore);
                TempData["MostraModal"] = "modalNuovoFornitore";
                
                return RedirectToAction(nameof(Fornitori));
            }

            try
            {
                if (fornitore.Id == 0)
                {
                    fornitore.DataCreazione = DateTime.Now;
                    _context.Fornitori.Add(fornitore);
                }
                else
                {
                    _context.Fornitori.Update(fornitore);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"✅ Fornitore '{fornitore.Nome}' salvato";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"❌ Errore: {ex.InnerException?.Message ?? ex.Message}";
                TempData["FornitoreCorrente"] = System.Text.Json.JsonSerializer.Serialize(fornitore);
                TempData["MostraModal"] = "modalNuovoFornitore";
            }

            return RedirectToAction(nameof(Fornitori));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminaFornitore(int id)
        {
            try
            {
                var fornitore = await _context.Fornitori.FindAsync(id);
                if (fornitore == null)
                {
                    TempData["ErrorMessage"] = "Fornitore non trovato";
                    return RedirectToAction(nameof(Fornitori));
                }

                var haForniture = await _context.ArticoliStock.AnyAsync(s => s.FornitoreId == id);
                if (haForniture)
                {
                    TempData["ErrorMessage"] = "❌ Impossibile eliminare: fornitore ha forniture in magazzino";
                    return RedirectToAction(nameof(Fornitori));
                }

                _context.Fornitori.Remove(fornitore);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"✅ Fornitore '{fornitore.Nome}' eliminato";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"❌ Errore: {ex.Message}";
            }

            return RedirectToAction(nameof(Fornitori));
        }

        // ========== CLIENTI ==========

        public async Task<IActionResult> Clienti()
        {
            var viewModel = new ClientiViewModel
            {
                Clienti = await _context.Clienti.OrderBy(c => c.Nome).ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvaCliente(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                // Raccogli errori di validazione
                var errori = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["ErrorMessage"] = "❌ Dati non validi: " + string.Join(", ", errori);
                TempData["ClienteCorrente"] = System.Text.Json.JsonSerializer.Serialize(cliente);
                TempData["MostraModal"] = "modalNuovoCliente";
                
                return RedirectToAction(nameof(Clienti));
            }

            try
            {
                if (cliente.Id == 0)
                {
                    cliente.DataCreazione = DateTime.Now;
                    _context.Clienti.Add(cliente);
                }
                else
                {
                    _context.Clienti.Update(cliente);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"✅ Cliente '{cliente.Nome}' salvato";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"❌ Errore: {ex.InnerException?.Message ?? ex.Message}";
                TempData["ClienteCorrente"] = System.Text.Json.JsonSerializer.Serialize(cliente);
                TempData["MostraModal"] = "modalNuovoCliente";
            }

            return RedirectToAction(nameof(Clienti));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminaCliente(int id)
        {
            try
            {
                var cliente = await _context.Clienti.FindAsync(id);
                if (cliente == null)
                {
                    TempData["ErrorMessage"] = "Cliente non trovato";
                    return RedirectToAction(nameof(Clienti));
                }

                var haOrdini = await _context.ArticoliStock.AnyAsync(s => s.ClienteId == id);
                if (haOrdini)
                {
                    TempData["ErrorMessage"] = "❌ Impossibile eliminare: cliente ha ordini in lavorazione";
                    return RedirectToAction(nameof(Clienti));
                }

                _context.Clienti.Remove(cliente);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"✅ Cliente '{cliente.Nome}' eliminato";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"❌ Errore: {ex.Message}";
            }

            return RedirectToAction(nameof(Clienti));
        }
    }
}