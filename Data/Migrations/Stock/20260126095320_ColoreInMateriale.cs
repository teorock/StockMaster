using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMaster.Data.Migrations.Stock
{
    /// <inheritdoc />
    public partial class ColoreInMateriale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColoreId",
                table: "Materiali",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materiali_ColoreId",
                table: "Materiali",
                column: "ColoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materiali_Colori_ColoreId",
                table: "Materiali",
                column: "ColoreId",
                principalTable: "Colori",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materiali_Colori_ColoreId",
                table: "Materiali");

            migrationBuilder.DropIndex(
                name: "IX_Materiali_ColoreId",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "ColoreId",
                table: "Materiali");
        }
    }
}
