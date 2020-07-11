using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V10UpdateSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HowItWorks",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OffersCountLimit",
                table: "Settings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TermsAndConditions",
                table: "Settings",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 11, 16, 25, 42, 552, DateTimeKind.Local).AddTicks(6315), 22501, "CE2B58BC50E5ACD670D06327898725AFCEC30D37895997017503A11A3A33A999372C8CF4AFEE817CB05DBEF4187D495B12535C897667B73DF11F7C08FFAD8C85", new byte[] { 114, 110, 86, 200, 120, 151, 30, 25, 155, 171, 239, 207, 42, 221, 6, 102, 179, 21, 10, 139, 93, 176, 109, 54, 68, 24, 173, 176, 104, 153, 51, 197 }, "6fcca5f2-729b-4a56-8eac-f14e682f67a6" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HowItWorks",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "OffersCountLimit",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "TermsAndConditions",
                table: "Settings");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 18, 19, 41, 42, 770, DateTimeKind.Local).AddTicks(7168), 62556, "796BD4B8E6CFE64B655DB67F015A081759B6822DDE3E8576B4454C3B979AD3ED9F4483F3AB73F13E7F7D0E4611D9A32291AAE278AEC2414E0375CF0169B93085", new byte[] { 112, 194, 97, 42, 229, 250, 0, 87, 24, 43, 98, 18, 16, 178, 67, 44, 74, 224, 121, 171, 50, 10, 31, 146, 30, 9, 149, 70, 95, 251, 65, 162 }, "a2c63b77-70d7-4abf-a50f-4cdc33d277f8" });
        }
    }
}
