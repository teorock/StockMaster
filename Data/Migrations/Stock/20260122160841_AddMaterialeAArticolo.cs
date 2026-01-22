using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMaster.Data.Migrations.Stock
{
    /// <inheritdoc />
    public partial class AddMaterialeAArticolo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Materiale",
                table: "Articoli",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Materiale",
                table: "Articoli");
        }
    }
}
