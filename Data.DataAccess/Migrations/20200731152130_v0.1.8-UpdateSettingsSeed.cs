using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class v018UpdateSettingsSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 1,
                column: "OffersCountLimit",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 31, 19, 51, 19, 330, DateTimeKind.Local).AddTicks(9717), 15984, "D2AE0651280BA28EAD6653F76D9113C169EC5E40497605456099D243FE39AE51614058C138FD97A27D50B26656BC49983F32E1682A0A513131224FB7AC388870", new byte[] { 93, 41, 22, 90, 208, 121, 121, 107, 110, 194, 103, 168, 127, 72, 189, 173, 190, 195, 206, 53, 125, 147, 242, 157, 5, 127, 2, 47, 57, 147, 85, 220 }, "2f36e564-d70c-43ad-aa41-854a9a272066" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 1,
                column: "OffersCountLimit",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 30, 17, 7, 17, 511, DateTimeKind.Local).AddTicks(8509), 80882, "C27833BB32434FE4FBE9928561E4C4EC5309E47F32AABAB14B863841C459E4AEDAC5ED9C43DB29F71264220F02D4E81C07C3A5E61530EA851FE8D1066D7910E7", new byte[] { 96, 68, 20, 15, 216, 177, 228, 3, 93, 240, 186, 61, 252, 122, 41, 119, 175, 226, 252, 135, 246, 72, 229, 180, 0, 155, 51, 47, 131, 77, 162, 106 }, "10705b9e-ac34-448b-88a2-14ea025ee8ad" });
        }
    }
}
