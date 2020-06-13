using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class AddProjectModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Beginning = table.Column<string>(nullable: false),
                    Destination = table.Column<string>(nullable: false),
                    Title = table.Column<string>(maxLength: 500, nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    Budget = table.Column<double>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Projects_MerchantId",
                table: "Projects",
                column: "MerchantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 11, 0, 36, 35, 320, DateTimeKind.Local).AddTicks(1150), 83323, "08F82D6D47F887F22B1C2B04AF79466ED5050E51199ECF285B7AAA39A3EBC4DA46005AC686EDEE9F37A52847BD56DE99BAC2FE2FE0753F6C2E020A645635B0DD", new byte[] { 125, 53, 148, 88, 1, 195, 51, 159, 126, 96, 7, 125, 52, 63, 188, 18, 191, 25, 33, 19, 128, 201, 97, 132, 239, 219, 123, 214, 50, 100, 128, 180 }, "e39485c6-12b1-44a1-9b65-0997bb581486" });
        }
    }
}
