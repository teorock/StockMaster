using Microsoft.EntityFrameworkCore;
using StockMaster.Models.Stock;

namespace StockMaster.Data
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options)
            : base(options)
        {
        }

        // DbSets per tutte le entità
        public DbSet<Articolo> Articoli { get; set; }
        public DbSet<Fornitore> Fornitori { get; set; }
        public DbSet<Cliente> Clienti { get; set; }
        public DbSet<FaseLavorazione> FasiLavorazione { get; set; }
        public DbSet<ArticoloStock> ArticoliStock { get; set; }
        public DbSet<TransizioneFase> TransizioniFase { get; set; }
        public DbSet<Materiale> Materiali { get; set; }
        public DbSet<Colore> Colori { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurazioni aggiuntive

            // Indici per performance
            modelBuilder.Entity<ArticoloStock>()
                .HasIndex(a => a.LottoRiferimento)
                .HasDatabaseName("IX_ArticoloStock_Lotto");

            modelBuilder.Entity<ArticoloStock>()
                .HasIndex(a => a.Fase)
                .HasDatabaseName("IX_ArticoloStock_Fase");

            modelBuilder.Entity<ArticoloStock>()
                .HasIndex(a => a.CodiceCommessa)
                .HasDatabaseName("IX_ArticoloStock_Commessa");

            modelBuilder.Entity<TransizioneFase>()
                .HasIndex(t => t.LottoRiferimento)
                .HasDatabaseName("IX_TransizioneFase_Lotto");

            modelBuilder.Entity<TransizioneFase>()
                .HasIndex(t => t.DataTransizione)
                .HasDatabaseName("IX_TransizioneFase_Data");

            // Articolo: Codice deve essere unique
            modelBuilder.Entity<Articolo>()
                .HasIndex(a => a.Codice)
                .IsUnique()
                .HasDatabaseName("IX_Articolo_Codice_Unique");

            // Seed dati iniziali - Fasi standard
            modelBuilder.Entity<FaseLavorazione>().HasData(
                new FaseLavorazione { Id = 1, Codice = "grezzo_stock", Descrizione = "Grezzo in Stock", OrdineSequenza = 1, Attivo = true },
                new FaseLavorazione { Id = 2, Codice = "lav_nesting", Descrizione = "Lavorazione Nesting", OrdineSequenza = 2, Attivo = true },
                new FaseLavorazione { Id = 3, Codice = "lav_bordatura", Descrizione = "Lavorazione Bordatura", OrdineSequenza = 3, Attivo = true },
                new FaseLavorazione { Id = 4, Codice = "verniciatura", Descrizione = "Verniciatura", OrdineSequenza = 4, Attivo = true },
                new FaseLavorazione { Id = 5, Codice = "rilavorazione", Descrizione = "Rilavorazione", OrdineSequenza = 5, Attivo = true },
                new FaseLavorazione { Id = 6, Codice = "finito_stock", Descrizione = "Finito in Stock", OrdineSequenza = 6, Attivo = true },
                new FaseLavorazione { Id = 7, Codice = "consegnato", Descrizione = "Consegnato", OrdineSequenza = 7, Attivo = true }
            );
        }
    }
}