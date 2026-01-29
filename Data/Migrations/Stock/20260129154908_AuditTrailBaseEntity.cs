using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMaster.Data.Migrations.Stock
{
    /// <inheritdoc />
    public partial class AuditTrailBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TransizioniFase",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "TransizioniFase",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "TransizioniFase",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "TransizioniFase",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Materiali",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Materiali",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Materiali",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Materiali",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Fornitori",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Fornitori",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Fornitori",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Fornitori",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "FasiLavorazione",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "FasiLavorazione",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "FasiLavorazione",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "FasiLavorazione",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Colori",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Colori",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Colori",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Colori",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Clienti",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Clienti",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Clienti",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Clienti",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ArticoliStock",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ArticoliStock",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ArticoliStock",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "ArticoliStock",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Articoli",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Articoli",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Articoli",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Articoli",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "FasiLavorazione",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null });

            migrationBuilder.UpdateData(
                table: "FasiLavorazione",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null });

            migrationBuilder.UpdateData(
                table: "FasiLavorazione",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null });

            migrationBuilder.UpdateData(
                table: "FasiLavorazione",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null });

            migrationBuilder.UpdateData(
                table: "FasiLavorazione",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null });

            migrationBuilder.UpdateData(
                table: "FasiLavorazione",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null });

            migrationBuilder.UpdateData(
                table: "FasiLavorazione",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TransizioniFase");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "TransizioniFase");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TransizioniFase");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "TransizioniFase");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Materiali");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Fornitori");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Fornitori");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Fornitori");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Fornitori");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "FasiLavorazione");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "FasiLavorazione");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "FasiLavorazione");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "FasiLavorazione");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Colori");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Colori");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Colori");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Colori");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Clienti");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Clienti");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Clienti");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Clienti");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ArticoliStock");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ArticoliStock");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ArticoliStock");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "ArticoliStock");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Articoli");
        }
    }
}
