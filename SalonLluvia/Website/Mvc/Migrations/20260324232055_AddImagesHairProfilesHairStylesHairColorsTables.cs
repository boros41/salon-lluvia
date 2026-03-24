using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddImagesHairProfilesHairStylesHairColorsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HairColors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HairColors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HairProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gender = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HairProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HairStyles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Style = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HairStyles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HairColorHairProfile",
                columns: table => new
                {
                    HairColorsId = table.Column<int>(type: "int", nullable: false),
                    HairProfilesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HairColorHairProfile", x => new { x.HairColorsId, x.HairProfilesId });
                    table.ForeignKey(
                        name: "FK_HairColorHairProfile_HairColors_HairColorsId",
                        column: x => x.HairColorsId,
                        principalTable: "HairColors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HairColorHairProfile_HairProfiles_HairProfilesId",
                        column: x => x.HairProfilesId,
                        principalTable: "HairProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    HairProfileId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_HairProfiles_HairProfileId",
                        column: x => x.HairProfileId,
                        principalTable: "HairProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HairProfileHairStyle",
                columns: table => new
                {
                    HairProfilesId = table.Column<int>(type: "int", nullable: false),
                    HairStylesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HairProfileHairStyle", x => new { x.HairProfilesId, x.HairStylesId });
                    table.ForeignKey(
                        name: "FK_HairProfileHairStyle_HairProfiles_HairProfilesId",
                        column: x => x.HairProfilesId,
                        principalTable: "HairProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HairProfileHairStyle_HairStyles_HairStylesId",
                        column: x => x.HairStylesId,
                        principalTable: "HairStyles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HairColorHairProfile_HairProfilesId",
                table: "HairColorHairProfile",
                column: "HairProfilesId");

            migrationBuilder.CreateIndex(
                name: "IX_HairProfileHairStyle_HairStylesId",
                table: "HairProfileHairStyle",
                column: "HairStylesId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_HairProfileId",
                table: "Images",
                column: "HairProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HairColorHairProfile");

            migrationBuilder.DropTable(
                name: "HairProfileHairStyle");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "HairColors");

            migrationBuilder.DropTable(
                name: "HairStyles");

            migrationBuilder.DropTable(
                name: "HairProfiles");
        }
    }
}
