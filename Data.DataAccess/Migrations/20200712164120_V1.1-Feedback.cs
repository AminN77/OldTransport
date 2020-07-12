using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V11Feedback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 12, 21, 11, 20, 187, DateTimeKind.Local).AddTicks(2885), 66368, "C55B43DD1A043081DDC0B13AF78D77C7C86071FCAEDE3111A3F57DDD4768542FF66A6B946CDB4DF6F865C180ED48145B46345D5F386F38C4A33F4B743607749D", new byte[] { 194, 226, 32, 217, 10, 153, 19, 109, 150, 255, 38, 237, 233, 155, 147, 220, 203, 245, 158, 169, 41, 91, 223, 212, 106, 31, 1, 153, 119, 127, 158, 149 }, "e8c3efc5-2caf-4fc0-8713-b05f9d2609e0" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 11, 16, 25, 42, 552, DateTimeKind.Local).AddTicks(6315), 22501, "CE2B58BC50E5ACD670D06327898725AFCEC30D37895997017503A11A3A33A999372C8CF4AFEE817CB05DBEF4187D495B12535C897667B73DF11F7C08FFAD8C85", new byte[] { 114, 110, 86, 200, 120, 151, 30, 25, 155, 171, 239, 207, 42, 221, 6, 102, 179, 21, 10, 139, 93, 176, 109, 54, 68, 24, 173, 176, 104, 153, 51, 197 }, "6fcca5f2-729b-4a56-8eac-f14e682f67a6" });
        }
    }
}
