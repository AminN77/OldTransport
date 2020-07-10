using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V10UpdateSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OffersCountLimit",
                table: "Settings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TermsAndConditions",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserGuide",
                table: "Settings",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 10, 16, 23, 10, 260, DateTimeKind.Local).AddTicks(9703), 68082, "13AB57F4E122A180F14E08C23D502BA0B1C2292C7C0BEA8704C88B02F6A00C540B71F920B4EA1A7534F8785DB7DC12D5E9FE6FED6C0100D13904BBA7273B1C96", new byte[] { 237, 55, 46, 143, 131, 44, 21, 162, 232, 166, 162, 186, 214, 39, 143, 118, 212, 198, 67, 69, 231, 57, 142, 24, 232, 147, 96, 175, 66, 95, 105, 12 }, "3610d601-690a-40c3-bbc9-98cfe66b49dc" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffersCountLimit",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "TermsAndConditions",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "UserGuide",
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
