using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class EditProjectModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_Beginning",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Destination",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Beginning",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "BeginningCity",
                table: "Projects",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BeginningCountry",
                table: "Projects",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DestinationCity",
                table: "Projects",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DestinationCountry",
                table: "Projects",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 14, 12, 17, 29, 943, DateTimeKind.Local).AddTicks(4831), 91312, "647DD12607A9B64930B741BCF2606EC30D464D3E15FB1270345726D504191E5CF6811BFE1E3E82770A5B8463537E513984A611686D14F84C5D0ED3142CE606A0", new byte[] { 196, 215, 143, 40, 77, 135, 54, 218, 173, 188, 30, 214, 211, 200, 170, 208, 105, 218, 110, 2, 32, 211, 204, 18, 194, 203, 0, 226, 141, 111, 205, 111 }, "596706c1-3f75-4722-9671-dde67b2252de" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BeginningCity",
                table: "Projects",
                column: "BeginningCity");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BeginningCountry",
                table: "Projects",
                column: "BeginningCountry");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DestinationCity",
                table: "Projects",
                column: "DestinationCity");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DestinationCountry",
                table: "Projects",
                column: "DestinationCountry");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_BeginningCity",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_BeginningCountry",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DestinationCity",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DestinationCountry",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "BeginningCity",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "BeginningCountry",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DestinationCity",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DestinationCountry",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "Beginning",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 13, 20, 55, 52, 163, DateTimeKind.Local).AddTicks(3470), 26877, "E9D000B4EF2BD8B9A4FB198CBCECD0F0FCF1B258E98435EFBBABC80916C6C044E70C81CC3802051B495897B0DFB71705BB82E2189684CB67BFB0AD69801F80FA", new byte[] { 236, 30, 33, 77, 181, 73, 128, 12, 182, 253, 33, 26, 219, 83, 146, 19, 228, 172, 234, 111, 125, 151, 17, 232, 212, 229, 165, 82, 15, 152, 84, 81 }, "6eed6564-58e3-455e-b3ca-edb990cf1e3d" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Beginning",
                table: "Projects",
                column: "Beginning");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Destination",
                table: "Projects",
                column: "Destination");
        }
    }
}
