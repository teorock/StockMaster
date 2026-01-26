using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMaster.Data.Migrations.Stock
{
    /// <inheritdoc />
    public partial class AggiuntaMateriali : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaterialeId",
                table: "Articoli",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Materiali",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCreazione = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiali", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articoli_MaterialeId",
                table: "Articoli",
                column: "MaterialeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articoli_Materiali_MaterialeId",
                table: "Articoli",
                column: "MaterialeId",
                principalTable: "Materiali",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articoli_Materiali_MaterialeId",
                table: "Articoli");

            migrationBuilder.DropTable(
                name: "Materiali");

            migrationBuilder.DropIndex(
                name: "IX_Articoli_MaterialeId",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "MaterialeId",
                table: "Articoli");
        }
    }
}
