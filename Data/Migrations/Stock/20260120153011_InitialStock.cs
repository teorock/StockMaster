using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StockMaster.Data.Migrations.Stock
{
    /// <inheritdoc />
    public partial class InitialStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articoli",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codice = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Descrizione = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    UnitaMisura = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    DataCreazione = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articoli", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clienti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PartitaIva = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Indirizzo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCreazione = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clienti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FasiLavorazione",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codice = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Descrizione = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OrdineSequenza = table.Column<int>(type: "INTEGER", nullable: false),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FasiLavorazione", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fornitori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PartitaIva = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Indirizzo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCreazione = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornitori", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransizioniFase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LottoRiferimento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FaseDa = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    FaseA = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Quantita = table.Column<int>(type: "INTEGER", nullable: false),
                    DataTransizione = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OperatoreId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransizioniFase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticoliStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArticoloId = table.Column<int>(type: "INTEGER", nullable: false),
                    CodiceArticolo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Fase = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Quantita = table.Column<int>(type: "INTEGER", nullable: false),
                    LottoRiferimento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FornitoreId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: true),
                    CodiceCommessa = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DataCreazione = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticoliStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticoliStock_Articoli_ArticoloId",
                        column: x => x.ArticoloId,
                        principalTable: "Articoli",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticoliStock_Clienti_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clienti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticoliStock_Fornitori_FornitoreId",
                        column: x => x.FornitoreId,
                        principalTable: "Fornitori",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "FasiLavorazione",
                columns: new[] { "Id", "Attivo", "Codice", "Descrizione", "Note", "OrdineSequenza" },
                values: new object[,]
                {
                    { 1, true, "grezzo_stock", "Grezzo in Stock", null, 1 },
                    { 2, true, "lav_nesting", "Lavorazione Nesting", null, 2 },
                    { 3, true, "lav_bordatura", "Lavorazione Bordatura", null, 3 },
                    { 4, true, "verniciatura", "Verniciatura", null, 4 },
                    { 5, true, "rilavorazione", "Rilavorazione", null, 5 },
                    { 6, true, "finito_stock", "Finito in Stock", null, 6 },
                    { 7, true, "consegnato", "Consegnato", null, 7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articolo_Codice_Unique",
                table: "Articoli",
                column: "Codice",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticoliStock_ArticoloId",
                table: "ArticoliStock",
                column: "ArticoloId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticoliStock_ClienteId",
                table: "ArticoliStock",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticoliStock_FornitoreId",
                table: "ArticoliStock",
                column: "FornitoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticoloStock_Commessa",
                table: "ArticoliStock",
                column: "CodiceCommessa");

            migrationBuilder.CreateIndex(
                name: "IX_ArticoloStock_Fase",
                table: "ArticoliStock",
                column: "Fase");

            migrationBuilder.CreateIndex(
                name: "IX_ArticoloStock_Lotto",
                table: "ArticoliStock",
                column: "LottoRiferimento");

            migrationBuilder.CreateIndex(
                name: "IX_TransizioneFase_Data",
                table: "TransizioniFase",
                column: "DataTransizione");

            migrationBuilder.CreateIndex(
                name: "IX_TransizioneFase_Lotto",
                table: "TransizioniFase",
                column: "LottoRiferimento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticoliStock");

            migrationBuilder.DropTable(
                name: "FasiLavorazione");

            migrationBuilder.DropTable(
                name: "TransizioniFase");

            migrationBuilder.DropTable(
                name: "Articoli");

            migrationBuilder.DropTable(
                name: "Clienti");

            migrationBuilder.DropTable(
                name: "Fornitori");
        }
    }
}
