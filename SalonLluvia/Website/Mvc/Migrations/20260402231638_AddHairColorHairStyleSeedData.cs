using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddHairColorHairStyleSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HairColors",
                columns: new[] { "Id", "Color" },
                values: new object[,]
                {
                    { 6, "azul" },
                    { 7, "morado" },
                    { 8, "rosa" }
                });

            migrationBuilder.UpdateData(
                table: "HairStyles",
                keyColumn: "Id",
                keyValue: 8,
                column: "Style",
                value: "liso");

            migrationBuilder.InsertData(
                table: "HairStyles",
                columns: new[] { "Id", "Style" },
                values: new object[] { 9, "otro" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HairColors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "HairColors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "HairColors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "HairStyles",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.UpdateData(
                table: "HairStyles",
                keyColumn: "Id",
                keyValue: 8,
                column: "Style",
                value: "otro");
        }
    }
}
