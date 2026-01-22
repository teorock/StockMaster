using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockMaster.Data;
using StockMaster.Services.Stock;
using StockMaster.ViewModels.Stock;

namespace StockMaster.Controllers
{
    public class MagazzinoController : Controller
    {
        private readonly StockDbContext _context;
        private readonly MagazzinoService _magazzinoService;

        public MagazzinoController(StockDbContext context, MagazzinoService magazzinoService)
        {
            _context = context;
            _magazzinoService = magazzinoService;
        }

        // GET: /Magazzino (Dashboard)
        public async Task<IActionResult> Index(int? clienteId)
        {
            var viewModel = new DashboardViewModel
            {
                ClienteSelezionatoId = clienteId
            };

            // Popola dropdown clienti
            viewModel.Clienti = await GetClientiConMaterialeSelectList();

            // Query base per lo stato magazzino
            var queryStock = _context.ArticoliStock
                .Where(s => s.Quantita > 0);

            // Applica filtro cliente se selezionato
            if (clienteId.HasValue)
            {
                queryStock = queryStock.Where(s => s.ClienteId == clienteId.Value);
            }

            // Stato per fase (con filtro cliente se applicato)
            var statoMagazzino = await queryStock
                .GroupBy(s => s.Fase)
                .Select(g => new StatoMagazzinoDto
                {
                    Fase = g.Key,
                    TotaleUnita = g.Sum(s => s.Quantita),
                    NumeroLotti = g.Select(s => s.LottoRiferimento).Distinct().Count()
                })
                .ToListAsync();

            viewModel.StatoPerFase = statoMagazzino.Select(s => new StatoFaseCard
            {
                Fase = s.Fase,
                FaseDescrizione = GetFaseDescrizione(s.Fase),
                TotaleUnita = s.TotaleUnita,
                NumeroLotti = s.NumeroLotti,
                CssClass = GetFaseCssClass(s.Fase)
            }).ToList();

            // Query per ultimi movimenti
            var queryLotti = await queryStock.Select(s => s.LottoRiferimento).Distinct().ToListAsync();

            // Ultimi 10 movimenti (filtrati per cliente se selezionato)
            var queryMovimenti = _context.TransizioniFase
                .Where(t => queryLotti.Contains(t.LottoRiferimento))
                .OrderByDescending(t => t.DataTransizione)
                .Take(10);

            viewModel.UltimiMovimenti = await queryMovimenti
                .Select(t => new UltimoMovimentoDto
                {
                    LottoRiferimento = t.LottoRiferimento,
                    FaseDa = t.FaseDa,
                    FaseA = t.FaseA,
                    Quantita = t.Quantita,
                    DataTransizione = t.DataTransizione,
                    Note = t.Note
                })
                .ToListAsync();

            // Totali (con filtro se applicato)
            viewModel.TotaleArticoliGestiti = await _context.Articoli.CountAsync();
            viewModel.TotaleLottiAttivi = await queryStock
                .Select(s => s.LottoRiferimento)
                .Distinct()
                .CountAsync();

            return View(viewModel);
        }

        private async Task<List<SelectListItem>> GetClientiConMaterialeSelectList()
        {
            var clientiConMateriale = await _context.ArticoliStock
                .Where(s => s.Quantita > 0 && s.ClienteId != null)
                .Select(s => new { s.ClienteId, s.Cliente!.Nome })
                .Distinct()
                .OrderBy(c => c.Nome)
                .ToListAsync();

            var items = clientiConMateriale.Select(c => new SelectListItem
            {
                Value = c.ClienteId.ToString(),
                Text = c.Nome
            }).ToList();

            items.Insert(0, new SelectListItem { Value = "", Text = "-- Tutti i clienti --" });
            return items;
        }

        // GET: /Magazzino/IngressoMerce
        public async Task<IActionResult> IngressoMerce()
        {
            var viewModel = new IngressoMerceViewModel
            {
                Articoli = await GetArticoliSelectList(),
                Fornitori = await GetFornitoriSelectList()
            };

            return View(viewModel);
        }

        // POST: /Magazzino/IngressoMerce
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IngressoMerce(IngressoMerceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Ripopola le dropdown prima di tornare alla view
                model.Articoli = await GetArticoliSelectList();
                model.Fornitori = await GetFornitoriSelectList();
                return View(model);
            }

            try
            {
                // Trova l'articolo
                var articolo = await _context.Articoli.FindAsync(model.ArticoloId);
                if (articolo == null)
                {
                    ModelState.AddModelError("", "Articolo non trovato");
                    model.Articoli = await GetArticoliSelectList();
                    model.Fornitori = await GetFornitoriSelectList();
                    return View(model);
                }

                // Registra l'ingresso
                var stock = await _magazzinoService.IngressoMerceAsync(
                    articoloId: model.ArticoloId,
                    codiceArticolo: articolo.Codice,
                    quantita: model.Quantita,
                    fornitoreId: model.FornitoreId,
                    note: model.Note,
                    codiceCommessa: model.CodiceCommessa
                );

                // Messaggio di successo
                TempData["SuccessMessage"] = $"✅ Ingresso registrato! Lotto: {stock.LottoRiferimento}, Quantità: {stock.Quantita} {articolo.UnitaMisura ?? "unità"}";
                
                // IMPORTANTE: Redirect esplicito alla Index
                return RedirectToAction("Index", "Magazzino");
            }
            catch (Exception ex)
            {
                // Log dell'errore per debugging
                ModelState.AddModelError("", $"Errore durante il salvataggio: {ex.Message}");
                
                // Ripopola le dropdown
                model.Articoli = await GetArticoliSelectList();
                model.Fornitori = await GetFornitoriSelectList();
                
                return View(model);
            }
        }

        /// <summary>
        /// API: Ottiene lista commesse attive con materiale in magazzino
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCommesseAttive(string term = "")
        {
            var query = _context.ArticoliStock
                .Where(s => s.Quantita > 0 && s.CodiceCommessa != null && s.CodiceCommessa != "");

            // Filtro per term (se fornito)
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(s => s.CodiceCommessa!.Contains(term));
            }

            var commesse = await query
                .GroupBy(s => new { s.CodiceCommessa, s.Cliente!.Nome })
                .Select(g => new
                {
                    codice = g.Key.CodiceCommessa,
                    cliente = g.Key.Nome ?? "N/D",
                    totaleUnita = g.Sum(s => s.Quantita),
                    numeroLotti = g.Select(s => s.LottoRiferimento).Distinct().Count()
                })
                .OrderByDescending(c => c.totaleUnita)
                .Take(20)
                .ToListAsync();

            return Json(commesse);
        }

        // GET: /Magazzino/MovimentoFase
        public async Task<IActionResult> MovimentoFase()
        {
            var viewModel = new MovimentoFaseViewModel
            {
                Lotti = await GetLottiAttiviSelectList(),
                Fasi = await GetFasiSelectList(),
                Clienti = await GetClientiSelectList()
            };

            return View(viewModel);
        }

        // POST: /Magazzino/MovimentoFase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MovimentoFase(MovimentoFaseViewModel model)
        {

            // PARSING: LottoRiferimento arriva come "LOTTO|FASE"
            // Estraiamo solo il LOTTO
            if (model.LottoRiferimento?.Contains('|') == true)
            {
                model.LottoRiferimento = model.LottoRiferimento.Split('|')[0];
            }

            if (!ModelState.IsValid)
            {
                model.Lotti = await GetLottiAttiviSelectList();
                model.Fasi = await GetFasiSelectList();
                model.Clienti = await GetClientiSelectList();
                return View(model);
            }

            try
            {
                // Validazione: se c'è scarto, deve esserci il motivo
                if (model.QuantitaScartata > 0 && string.IsNullOrWhiteSpace(model.MotivoScarto))
                {
                    ModelState.AddModelError("MotivoScarto", "Il motivo dello scarto è obbligatorio quando si scarta materiale");
                    model.Lotti = await GetLottiAttiviSelectList();
                    model.Fasi = await GetFasiSelectList();
                    model.Clienti = await GetClientiSelectList();
                    return View(model);
                }

                // Validazione: quantità totale deve essere > 0
                if (model.Quantita == 0 && model.QuantitaScartata == 0)
                {
                    ModelState.AddModelError("", "Devi indicare una quantità da spostare o da scartare");
                    model.Lotti = await GetLottiAttiviSelectList();
                    model.Fasi = await GetFasiSelectList();
                    model.Clienti = await GetClientiSelectList();
                    return View(model);
                }

                // Chiama il service appropriato
                if (model.QuantitaScartata > 0)
                {
                    // Movimento con scarto
                    await _magazzinoService.SpostaFaseConScartoAsync(
                        lottoRiferimento: model.LottoRiferimento,
                        faseDa: model.FaseDa,
                        faseA: model.FaseA,
                        quantita: model.Quantita,
                        quantitaScartata: model.QuantitaScartata,
                        motivoScarto: model.MotivoScarto,
                        clienteId: model.ClienteId,
                        codiceCommessa: model.CodiceCommessa,
                        operatoreId: User.Identity?.Name,
                        note: model.Note
                    );

                    TempData["SuccessMessage"] = $"✅ Movimento registrato! {model.Quantita} unità → {model.FaseA}, {model.QuantitaScartata} scartate";
                }
                else
                {
                    // Movimento normale (senza scarto)
                    await _magazzinoService.SpostaFaseAsync(
                        lottoRiferimento: model.LottoRiferimento,
                        faseDa: model.FaseDa,
                        faseA: model.FaseA,
                        quantita: model.Quantita,
                        clienteId: model.ClienteId,
                        codiceCommessa: model.CodiceCommessa,
                        operatoreId: User.Identity?.Name,
                        note: model.Note
                    );

                    TempData["SuccessMessage"] = $"✅ Movimento registrato! {model.Quantita} unità da {model.FaseDa} → {model.FaseA}";
                }

                return RedirectToAction("Index", "Magazzino");
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 5)
            {
                ModelState.AddModelError("", "⚠️ Database occupato! Chiudi DbBrowser o altri programmi che stanno usando il database e riprova.");
                model.Lotti = await GetLottiAttiviSelectList();
                model.Fasi = await GetFasiSelectList();
                model.Clienti = await GetClientiSelectList();
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", $"❌ {ex.Message}");
                model.Lotti = await GetLottiAttiviSelectList();
                model.Fasi = await GetFasiSelectList();
                model.Clienti = await GetClientiSelectList();
                return View(model);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", $"❌ Errore database: {ex.InnerException?.Message ?? ex.Message}");
                model.Lotti = await GetLottiAttiviSelectList();
                model.Fasi = await GetFasiSelectList();
                model.Clienti = await GetClientiSelectList();
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"❌ Errore imprevisto: {ex.Message}");
                model.Lotti = await GetLottiAttiviSelectList();
                model.Fasi = await GetFasiSelectList();
                model.Clienti = await GetClientiSelectList();
                return View(model);
            }
        }

        // GET: /Magazzino/DettaglioCommessa?codice=ORD_2025_001
        public async Task<IActionResult> DettaglioCommessa(string codice)
        {
            if (string.IsNullOrWhiteSpace(codice))
            {
                TempData["ErrorMessage"] = "Codice commessa non specificato";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new DettaglioCommessaViewModel
            {
                CodiceCommessa = codice
            };

            // Ottieni tutti i lotti associati alla commessa
            var lottiCommessa = await _context.ArticoliStock
                .Include(s => s.Articolo)
                .Include(s => s.Cliente)
                .Where(s => s.CodiceCommessa == codice && s.Quantita > 0)
                .OrderBy(s => s.LottoRiferimento)
                .ThenBy(s => s.Fase)
                .ToListAsync();

            if (!lottiCommessa.Any())
            {
                TempData["ErrorMessage"] = $"Nessun materiale trovato per la commessa {codice}";
                return RedirectToAction(nameof(Index));
            }

            // Popola lotti
            viewModel.Lotti = lottiCommessa.Select(s => new LottoCommessaDto
            {
                LottoRiferimento = s.LottoRiferimento,
                CodiceArticolo = s.CodiceArticolo,
                DescrizioneArticolo = s.Articolo?.Descrizione ?? "",
                Fase = s.Fase,
                Quantita = s.Quantita,
                DataCreazione = s.DataCreazione,
                Note = s.Note
            }).ToList();

            // Cliente (prendi dal primo record)
            var primoRecord = lottiCommessa.First();
            viewModel.NomeCliente = primoRecord.Cliente?.Nome;
            viewModel.ClienteId = primoRecord.ClienteId;

            // Statistiche
            viewModel.TotaleLotti = lottiCommessa.Select(s => s.LottoRiferimento).Distinct().Count();
            viewModel.TotaleUnita = lottiCommessa.Sum(s => s.Quantita);
            viewModel.UnitaConsegnate = lottiCommessa.Where(s => s.Fase == "consegnato").Sum(s => s.Quantita);

            // Percentuale completamento (semplificata)
            if (viewModel.TotaleUnita > 0)
            {
                viewModel.PercentualeCompletamento = (decimal)viewModel.UnitaConsegnate / viewModel.TotaleUnita * 100;
            }

            // Date
            viewModel.DataPrimoIngresso = lottiCommessa.Min(s => s.DataCreazione);
            viewModel.DataUltimoMovimento = await _context.TransizioniFase
                .Where(t => lottiCommessa.Select(l => l.LottoRiferimento).Contains(t.LottoRiferimento))
                .MaxAsync(t => (DateTime?)t.DataTransizione);

            return View(viewModel);
        }

        // GET: /Magazzino/CercaCommessa (form di ricerca)
        public IActionResult CercaCommessa()
        {
            return View();
        }

        // POST: /Magazzino/CercaCommessa
        [HttpPost]
        public IActionResult CercaCommessa(string codiceCommessa)
        {
            if (string.IsNullOrWhiteSpace(codiceCommessa))
            {
                ModelState.AddModelError("", "Inserisci un codice commessa");
                return View();
            }

            return RedirectToAction(nameof(DettaglioCommessa), new { codice = codiceCommessa });
        }

        /// <summary>
        /// API: Ottiene informazioni su un lotto per auto-compilare il form MovimentoFase
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetInfoLotto(string lotto)
        {
            try
            {
                var stockInfo = await _context.ArticoliStock
                    .Include(s => s.Articolo)
                    .Where(s => s.LottoRiferimento == lotto && s.Quantita > 0)
                    .Select(s => new
                    {
                        CodiceArticolo = s.CodiceArticolo,
                        DescrizioneArticolo = s.Articolo!.Descrizione,
                        Fase = s.Fase,
                        Quantita = s.Quantita,
                        ClienteId = s.ClienteId,
                        CodiceCommessa = s.CodiceCommessa
                    })
                    .ToListAsync();

                if (!stockInfo.Any())
                {
                    return Json(new { success = false, message = "Lotto non trovato o esaurito" });
                }

                // Se il lotto è in più fasi, prendi la prima (tipicamente sarà una sola)
                var info = stockInfo.First();

                return Json(new
                {
                    success = true,
                    codiceArticolo = info.CodiceArticolo,
                    descrizioneArticolo = info.DescrizioneArticolo,
                    fase = info.Fase,
                    quantita = info.Quantita,
                    clienteId = info.ClienteId,
                    codiceCommessa = info.CodiceCommessa,
                    fasi = stockInfo.Select(s => new { fase = s.Fase, quantita = s.Quantita }).ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // GET: /Magazzino/DettaglioLotto/{lotto}
        public async Task<IActionResult> DettaglioLotto(string lotto)
        {
            var dettaglio = await _magazzinoService.GetDettaglioLottoAsync(lotto);
            var storico = await _magazzinoService.GetStoricoLottoAsync(lotto);

            ViewBag.Lotto = lotto;
            ViewBag.Storico = storico;

            return View(dettaglio);
        }

        // Helper methods per popolare dropdown
        private async Task<List<SelectListItem>> GetArticoliSelectList()
        {
            return await _context.Articoli
                .OrderBy(a => a.Codice)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.Codice} - {a.Descrizione}"
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetFornitoriSelectList()
        {
            return await _context.Fornitori
                .Where(f => f.Attivo)
                .OrderBy(f => f.Nome)
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Nome
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetClientiSelectList()
        {
            var items = await _context.Clienti
                .Where(c => c.Attivo)
                .OrderBy(c => c.Nome)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nome
                })
                .ToListAsync();

            items.Insert(0, new SelectListItem { Value = "", Text = "-- Nessuno --" });
            return items;
        }

        private async Task<List<SelectListItem>> GetLottiAttiviSelectList()
        {
            var lotti = await _context.ArticoliStock
                .Where(s => s.Quantita > 0)
                .Include(s => s.Articolo)
                .OrderByDescending(s => s.DataCreazione)
                .ThenBy(s => s.Fase)
                .ToListAsync();

            return lotti.Select(s => new SelectListItem
            {
                // Value: "LOTTO|FASE" (così sappiamo cosa ha scelto l'utente)
                Value = $"{s.LottoRiferimento}|{s.Fase}",
                
                // Text: quello che vede l'utente (come prima)
                Text = $"{s.LottoRiferimento} - {s.Articolo!.Codice} ({s.Fase}, {s.Quantita} pz)"
            }).ToList();
        }

        private async Task<List<SelectListItem>> GetFasiSelectList()
        {
            return await _context.FasiLavorazione
                .Where(f => f.Attivo)
                .OrderBy(f => f.OrdineSequenza)
                .Select(f => new SelectListItem
                {
                    Value = f.Codice,
                    Text = f.Descrizione
                })
                .ToListAsync();
        }

        // Helper per descrizioni e stili
        private string GetFaseDescrizione(string codice)
        {
            return codice switch
            {
                "grezzo_stock" => "Grezzo in Stock",
                "lav_nesting" => "Lavorazione Nesting",
                "lav_bordatura" => "Lavorazione Bordatura",
                "verniciatura" => "Verniciatura",
                "rilavorazione" => "Rilavorazione",
                "finito_stock" => "Finito in Stock",
                "consegnato" => "Consegnato",
                _ => codice
            };
        }

        private string GetFaseCssClass(string fase)
        {
            return fase switch
            {
                "grezzo_stock" => "secondary",
                "lav_nesting" => "info",
                "lav_bordatura" => "primary",
                "verniciatura" => "warning",
                "rilavorazione" => "danger",
                "finito_stock" => "success",
                "consegnato" => "dark",
                _ => "secondary"
            };
        }
    }
}