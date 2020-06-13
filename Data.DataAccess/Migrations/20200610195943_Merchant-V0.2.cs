using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class MerchantV02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Bio = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Merchants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "User");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "DeveloperSupport");

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { 1, 3 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 11, 0, 29, 42, 387, DateTimeKind.Local).AddTicks(3535), 92408, "B22FFD5992F0AC8CC27C0D00DBFCCAB7958B989E34076777D929FC67CF5018B00EAE9B4EF97A5B20A335C15342F8036C39247A082529DDB2E9B9DCF109648339", new byte[] { 194, 178, 184, 28, 220, 100, 161, 186, 0, 60, 7, 91, 63, 90, 27, 131, 12, 249, 127, 30, 125, 119, 173, 207, 122, 0, 47, 137, 196, 25, 200, 126 }, "2cb1689c-2cf9-4605-abb7-4b1f3dd16c3d" });

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_UserId",
                table: "Merchants",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Merchant");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Transporter");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "DeveloperSupport" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 5, 10, 21, 44, 54, 577, DateTimeKind.Local).AddTicks(3575), 12282, "119887D9D56B62BB7A97A3A781F1B1436150A43C6E268B1407A3EBABEA799DAE9EAACA7D90C75D5E65AFAE4FC5E9CAF666FF2C1FF8EFFB78890050924092B76F", new byte[] { 21, 227, 140, 104, 147, 161, 197, 247, 166, 236, 36, 253, 32, 229, 107, 36, 110, 193, 144, 69, 234, 177, 201, 209, 21, 200, 245, 160, 235, 27, 176, 45 }, "7dccb370-470d-4b80-a68e-fe3553c1dc7d" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { 1, 4 });
        }
    }
}
