using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mvc.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialHairStyleHairColorData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HairColors",
                columns: new[] { "Id", "Color" },
                values: new object[,]
                {
                    { 1, "negro" },
                    { 2, "cafe" },
                    { 3, "rubio" },
                    { 4, "rojo" },
                    { 5, "mixto" }
                });

            migrationBuilder.InsertData(
                table: "HairStyles",
                columns: new[] { "Id", "Style" },
                values: new object[,]
                {
                    { 1, "peinado" },
                    { 2, "tinte" },
                    { 3, "chino" },
                    { 4, "trenzas" },
                    { 5, "corte" },
                    { 6, "largo" },
                    { 7, "corto" },
                    { 8, "otro" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HairColors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "HairColors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "HairColors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "HairColors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "HairColors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "HairStyles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "HairStyles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "HairStyles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "HairStyles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "HairStyles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "HairStyles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "HairStyles",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "HairStyles",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
