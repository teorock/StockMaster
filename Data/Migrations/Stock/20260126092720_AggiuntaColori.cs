using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMaster.Data.Migrations.Stock
{
    /// <inheritdoc />
    public partial class AggiuntaColori : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColoreId",
                table: "Articoli",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Colori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CodiceHex = table.Column<string>(type: "TEXT", maxLength: 7, nullable: true),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCreazione = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colori", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articoli_ColoreId",
                table: "Articoli",
                column: "ColoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articoli_Colori_ColoreId",
                table: "Articoli",
                column: "ColoreId",
                principalTable: "Colori",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articoli_Colori_ColoreId",
                table: "Articoli");

            migrationBuilder.DropTable(
                name: "Colori");

            migrationBuilder.DropIndex(
                name: "IX_Articoli_ColoreId",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "ColoreId",
                table: "Articoli");
        }
    }
}
