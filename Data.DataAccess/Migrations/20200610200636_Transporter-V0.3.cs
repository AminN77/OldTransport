using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class TransporterV03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transporters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Bio = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transporters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transporters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 11, 0, 36, 35, 320, DateTimeKind.Local).AddTicks(1150), 83323, "08F82D6D47F887F22B1C2B04AF79466ED5050E51199ECF285B7AAA39A3EBC4DA46005AC686EDEE9F37A52847BD56DE99BAC2FE2FE0753F6C2E020A645635B0DD", new byte[] { 125, 53, 148, 88, 1, 195, 51, 159, 126, 96, 7, 125, 52, 63, 188, 18, 191, 25, 33, 19, 128, 201, 97, 132, 239, 219, 123, 214, 50, 100, 128, 180 }, "e39485c6-12b1-44a1-9b65-0997bb581486" });

            migrationBuilder.CreateIndex(
                name: "IX_Transporters_UserId",
                table: "Transporters",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transporters");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 11, 0, 29, 42, 387, DateTimeKind.Local).AddTicks(3535), 92408, "B22FFD5992F0AC8CC27C0D00DBFCCAB7958B989E34076777D929FC67CF5018B00EAE9B4EF97A5B20A335C15342F8036C39247A082529DDB2E9B9DCF109648339", new byte[] { 194, 178, 184, 28, 220, 100, 161, 186, 0, 60, 7, 91, 63, 90, 27, 131, 12, 249, 127, 30, 125, 119, 173, 207, 122, 0, 47, 137, 196, 25, 200, 126 }, "2cb1689c-2cf9-4605-abb7-4b1f3dd16c3d" });
        }
    }
}
