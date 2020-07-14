using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V09UpdateUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 18, 19, 41, 42, 770, DateTimeKind.Local).AddTicks(7168), 62556, "796BD4B8E6CFE64B655DB67F015A081759B6822DDE3E8576B4454C3B979AD3ED9F4483F3AB73F13E7F7D0E4611D9A32291AAE278AEC2414E0375CF0169B93085", new byte[] { 112, 194, 97, 42, 229, 250, 0, 87, 24, 43, 98, 18, 16, 178, 67, 44, 74, 224, 121, 171, 50, 10, 31, 146, 30, 9, 149, 70, 95, 251, 65, 162 }, "a2c63b77-70d7-4abf-a50f-4cdc33d277f8" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 16, 15, 35, 56, 977, DateTimeKind.Local).AddTicks(5636), 84929, "1B3A5EB00E11506A015FFE16F2CC1CBC0D26D47EAE6E333B9BB51240FF03D503A8B38AFE22E28BD27E7B660F63538A2068090A57E11C977722D8F611BEAD26C4", new byte[] { 220, 45, 236, 69, 99, 182, 13, 211, 23, 206, 195, 244, 220, 40, 82, 166, 215, 126, 213, 91, 0, 212, 189, 24, 206, 175, 172, 35, 154, 217, 113, 120 }, "b3a6906e-4806-4eea-b96c-a776d6165446" });
        }
    }
}
