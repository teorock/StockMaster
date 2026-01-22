using Microsoft.EntityFrameworkCore;
using StockMaster.Data;
using StockMaster.Models.Stock;

namespace StockMaster.Services.Stock
{
    /// <summary>
    /// Service per la gestione del magazzino
    /// Gestisce ingresso merce, movimenti tra fasi, transazioni atomiche
    /// </summary>
    public class MagazzinoService
    {
        private readonly StockDbContext _context;

        public MagazzinoService(StockDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registra l'ingresso di nuova merce da fornitore
        /// </summary>
        public async Task<ArticoloStock> IngressoMerceAsync(
            int articoloId,
            string codiceArticolo,
            int quantita,
            int fornitoreId,
            string? note = null,
            string? codiceCommessa = null)
        {
            // Genera codice lotto univoco
            var lottoRiferimento = await GeneraCodiceLottoAsync(codiceArticolo);

            // Crea record stock in fase "grezzo_stock"
            var stock = new ArticoloStock
            {
                ArticoloId = articoloId,
                CodiceArticolo = codiceArticolo,
                Fase = "grezzo_stock",
                Quantita = quantita,
                LottoRiferimento = lottoRiferimento,
                FornitoreId = fornitoreId,
                CodiceCommessa = codiceCommessa,
                DataCreazione = DateTime.Now,
                Note = note
            };

            // Crea transizione (da NULL a grezzo_stock = ingresso)
            var transizione = new TransizioneFase
            {
                LottoRiferimento = lottoRiferimento,
                FaseDa = null, // NULL = ingresso iniziale
                FaseA = "grezzo_stock",
                Quantita = quantita,
                DataTransizione = DateTime.Now,
                Note = note ?? $"Ingresso da fornitore ID {fornitoreId}"
            };

            // Salva in transazione
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.ArticoliStock.Add(stock);
                    _context.TransizioniFase.Add(transizione);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return stock;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// Sposta materiale da una fase all'altra
        /// </summary>
        public async Task<bool> SpostaFaseAsync(
            string lottoRiferimento,
            string faseDa,
            string faseA,
            int quantita,
            int? clienteId = null,
            string? codiceCommessa = null,
            string? operatoreId = null,
            string? note = null)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Trova il record nella fase di partenza
                    var stockPartenza = await _context.ArticoliStock
                        .FirstOrDefaultAsync(s => 
                            s.LottoRiferimento == lottoRiferimento && 
                            s.Fase == faseDa);

                    if (stockPartenza == null)
                    {
                        throw new InvalidOperationException(
                            $"Nessun materiale trovato per lotto {lottoRiferimento} in fase {faseDa}");
                    }

                    if (stockPartenza.Quantita < quantita)
                    {
                        throw new InvalidOperationException(
                            $"Quantità insufficiente. Disponibili: {stockPartenza.Quantita}, Richieste: {quantita}");
                    }

                    // 2. Riduci quantità in fase partenza
                    stockPartenza.Quantita -= quantita;

                    // Se quantità diventa 0, elimina il record (opzionale)
                    if (stockPartenza.Quantita == 0)
                    {
                        _context.ArticoliStock.Remove(stockPartenza);
                    }

                    // 3. Cerca se esiste già un record nella fase di arrivo per questo lotto
                    var stockArrivo = await _context.ArticoliStock
                        .FirstOrDefaultAsync(s => 
                            s.LottoRiferimento == lottoRiferimento && 
                            s.Fase == faseA);

                    if (stockArrivo != null)
                    {
                        // Esiste già: incrementa quantità
                        stockArrivo.Quantita += quantita;
                        
                        // Aggiorna cliente/commessa se specificati
                        if (clienteId.HasValue)
                            stockArrivo.ClienteId = clienteId;
                        if (!string.IsNullOrEmpty(codiceCommessa))
                            stockArrivo.CodiceCommessa = codiceCommessa;
                    }
                    else
                    {
                        // Non esiste: crea nuovo record
                        stockArrivo = new ArticoloStock
                        {
                            ArticoloId = stockPartenza.ArticoloId,
                            CodiceArticolo = stockPartenza.CodiceArticolo,
                            Fase = faseA,
                            Quantita = quantita,
                            LottoRiferimento = lottoRiferimento,
                            FornitoreId = stockPartenza.FornitoreId,
                            ClienteId = clienteId,
                            CodiceCommessa = codiceCommessa ?? stockPartenza.CodiceCommessa,
                            DataCreazione = DateTime.Now,
                            Note = note
                        };
                        _context.ArticoliStock.Add(stockArrivo);
                    }

                    // 4. Registra la transizione
                    var transizione = new TransizioneFase
                    {
                        LottoRiferimento = lottoRiferimento,
                        FaseDa = faseDa,
                        FaseA = faseA,
                        Quantita = quantita,
                        DataTransizione = DateTime.Now,
                        OperatoreId = operatoreId,
                        Note = note
                    };
                    _context.TransizioniFase.Add(transizione);

                    // 5. Salva tutto
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// Sposta materiale da una fase all'altra CON possibilità di scarto parziale
        /// </summary>
        public async Task<bool> SpostaFaseConScartoAsync(
            string lottoRiferimento,
            string faseDa,
            string faseA,
            int quantita,
            int quantitaScartata = 0,
            string? motivoScarto = null,
            int? clienteId = null,
            string? codiceCommessa = null,
            string? operatoreId = null,
            string? note = null)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var stockPartenza = await _context.ArticoliStock
                        .FirstOrDefaultAsync(s => 
                            s.LottoRiferimento == lottoRiferimento && 
                            s.Fase == faseDa);

                    if (stockPartenza == null)
                    {
                        throw new InvalidOperationException(
                            $"Nessun materiale trovato per lotto {lottoRiferimento} in fase {faseDa}");
                    }

                    var quantitaTotale = quantita + quantitaScartata;

                    if (stockPartenza.Quantita < quantitaTotale)
                    {
                        throw new InvalidOperationException(
                            $"Quantità insufficiente. Disponibili: {stockPartenza.Quantita}, Richieste: {quantitaTotale} (di cui {quantitaScartata} scarto)");
                    }

                    // 1. Riduci quantità totale
                    stockPartenza.Quantita -= quantitaTotale;

                    if (stockPartenza.Quantita == 0)
                    {
                        _context.ArticoliStock.Remove(stockPartenza);
                    }

                    // 2. Sposta quantità buona
                    if (quantita > 0)
                    {
                        var stockArrivo = await _context.ArticoliStock
                            .FirstOrDefaultAsync(s => 
                                s.LottoRiferimento == lottoRiferimento && 
                                s.Fase == faseA);

                        if (stockArrivo != null)
                        {
                            stockArrivo.Quantita += quantita;
                            if (clienteId.HasValue)
                                stockArrivo.ClienteId = clienteId;
                            if (!string.IsNullOrEmpty(codiceCommessa))
                                stockArrivo.CodiceCommessa = codiceCommessa;
                        }
                        else
                        {
                            stockArrivo = new ArticoloStock
                            {
                                ArticoloId = stockPartenza.ArticoloId,
                                CodiceArticolo = stockPartenza.CodiceArticolo,
                                Fase = faseA,
                                Quantita = quantita,
                                LottoRiferimento = lottoRiferimento,
                                FornitoreId = stockPartenza.FornitoreId,
                                ClienteId = clienteId,
                                CodiceCommessa = codiceCommessa ?? stockPartenza.CodiceCommessa,
                                DataCreazione = DateTime.Now,
                                Note = note
                            };
                            _context.ArticoliStock.Add(stockArrivo);
                        }

                        // Transizione materiale buono
                        var transizioneOk = new TransizioneFase
                        {
                            LottoRiferimento = lottoRiferimento,
                            FaseDa = faseDa,
                            FaseA = faseA,
                            Quantita = quantita,
                            DataTransizione = DateTime.Now,
                            OperatoreId = operatoreId,
                            Note = note
                        };
                        _context.TransizioniFase.Add(transizioneOk);
                    }

                    // 3. Gestisci scarti
                    if (quantitaScartata > 0)
                    {
                        // Registra in fase scarto (per statistiche)
                        var stockScarto = new ArticoloStock
                        {
                            ArticoloId = stockPartenza.ArticoloId,
                            CodiceArticolo = stockPartenza.CodiceArticolo,
                            Fase = "scarto",
                            Quantita = quantitaScartata,
                            LottoRiferimento = lottoRiferimento,
                            FornitoreId = stockPartenza.FornitoreId,
                            ClienteId = stockPartenza.ClienteId,
                            CodiceCommessa = stockPartenza.CodiceCommessa,
                            DataCreazione = DateTime.Now,
                            Note = motivoScarto
                        };
                        _context.ArticoliStock.Add(stockScarto);

                        // Transizione scarto
                        var transizioneScarto = new TransizioneFase
                        {
                            LottoRiferimento = lottoRiferimento,
                            FaseDa = faseDa,
                            FaseA = "scarto",
                            Quantita = quantitaScartata,
                            DataTransizione = DateTime.Now,
                            OperatoreId = operatoreId,
                            Note = $"SCARTO: {motivoScarto}"
                        };
                        _context.TransizioniFase.Add(transizioneScarto);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }


        /// <summary>
        /// Scarta materiale (esce definitivamente dal processo produttivo)
        /// </summary>
        public async Task<bool> ScartaMaterialeAsync(
            string lottoRiferimento,
            string faseDa,
            int quantita,
            string motivoScarto,
            string? operatoreId = null)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Trova il record nella fase di partenza
                    var stockPartenza = await _context.ArticoliStock
                        .FirstOrDefaultAsync(s => 
                            s.LottoRiferimento == lottoRiferimento && 
                            s.Fase == faseDa);

                    if (stockPartenza == null)
                    {
                        throw new InvalidOperationException(
                            $"Nessun materiale trovato per lotto {lottoRiferimento} in fase {faseDa}");
                    }

                    if (stockPartenza.Quantita < quantita)
                    {
                        throw new InvalidOperationException(
                            $"Quantità insufficiente. Disponibili: {stockPartenza.Quantita}, Da scartare: {quantita}");
                    }

                    // 2. Riduci quantità in fase partenza
                    stockPartenza.Quantita -= quantita;

                    if (stockPartenza.Quantita == 0)
                    {
                        _context.ArticoliStock.Remove(stockPartenza);
                    }

                    // 3. OPZIONALE: Crea record in fase "scarto" per statistiche
                    // (puoi commentare se non vuoi vedere scarti in dashboard)
                    var stockScarto = new ArticoloStock
                    {
                        ArticoloId = stockPartenza.ArticoloId,
                        CodiceArticolo = stockPartenza.CodiceArticolo,
                        Fase = "scarto",
                        Quantita = quantita,
                        LottoRiferimento = lottoRiferimento,
                        FornitoreId = stockPartenza.FornitoreId,
                        ClienteId = stockPartenza.ClienteId,
                        CodiceCommessa = stockPartenza.CodiceCommessa,
                        DataCreazione = DateTime.Now,
                        Note = motivoScarto
                    };
                    _context.ArticoliStock.Add(stockScarto);

                    // 4. Registra la transizione
                    var transizione = new TransizioneFase
                    {
                        LottoRiferimento = lottoRiferimento,
                        FaseDa = faseDa,
                        FaseA = "scarto",
                        Quantita = quantita,
                        DataTransizione = DateTime.Now,
                        OperatoreId = operatoreId,
                        Note = $"SCARTO: {motivoScarto}"
                    };
                    _context.TransizioniFase.Add(transizione);

                    // 5. Salva tutto
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// Ottiene lo stato corrente del magazzino raggruppato per fase
        /// </summary>
        public async Task<List<StatoMagazzinoDto>> GetStatoMagazzinoAsync()
        {
            var stato = await _context.ArticoliStock
                .Where(s => s.Quantita > 0)
                .GroupBy(s => s.Fase)
                .Select(g => new StatoMagazzinoDto
                {
                    Fase = g.Key,
                    TotaleUnita = g.Sum(s => s.Quantita),
                    NumeroLotti = g.Select(s => s.LottoRiferimento).Distinct().Count()
                })
                .ToListAsync();

            return stato;
        }

        /// <summary>
        /// Ottiene lo storico completo di un lotto
        /// </summary>
        public async Task<List<TransizioneFase>> GetStoricoLottoAsync(string lottoRiferimento)
        {
            return await _context.TransizioniFase
                .Where(t => t.LottoRiferimento == lottoRiferimento)
                .OrderBy(t => t.DataTransizione)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene il dettaglio stock di un lotto specifico
        /// </summary>
        public async Task<List<ArticoloStock>> GetDettaglioLottoAsync(string lottoRiferimento)
        {
            return await _context.ArticoliStock
                .Include(s => s.Articolo)
                .Include(s => s.Fornitore)
                .Include(s => s.Cliente)
                .Where(s => s.LottoRiferimento == lottoRiferimento && s.Quantita > 0)
                .ToListAsync();
        }

        /// <summary>
        /// Genera un codice lotto univoco
        /// Formato: LOTTO_{ANNO}_{CODICE_ARTICOLO}_{PROGRESSIVO}
        /// </summary>
        private async Task<string> GeneraCodiceLottoAsync(string codiceArticolo)
        {
            var anno = DateTime.Now.Year;
            var prefisso = $"LOTTO_{anno}_{codiceArticolo}";

            // Conta quanti lotti esistono già con questo prefisso
            var count = await _context.ArticoliStock
                .Where(s => s.LottoRiferimento.StartsWith(prefisso))
                .Select(s => s.LottoRiferimento)
                .Distinct()
                .CountAsync();

            var progressivo = (count + 1).ToString("D3"); // 001, 002, etc.

            return $"{prefisso}_{progressivo}";
        }
    }



    /// <summary>
    /// DTO per rappresentare lo stato del magazzino
    /// </summary>
    public class StatoMagazzinoDto
    {
        public string Fase { get; set; } = string.Empty;
        public int TotaleUnita { get; set; }
        public int NumeroLotti { get; set; }
    }
}