using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V12ContactUs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 12, 22, 29, 43, 408, DateTimeKind.Local).AddTicks(7884), 83064, "262C4E4169FA263CC4184CDC9B0A358EF176B65B0EC662BDCE48FE196013571CE140552BEBCC09417DAD4C590D85B1ADFB9612A5F6E9A8536C1D016229297668", new byte[] { 189, 91, 78, 44, 230, 193, 180, 89, 74, 139, 182, 28, 45, 132, 128, 87, 201, 105, 218, 78, 131, 20, 189, 210, 106, 235, 247, 94, 137, 162, 205, 57 }, "9b8a53cf-7df1-46a7-9838-efe78366f198" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 12, 21, 11, 20, 187, DateTimeKind.Local).AddTicks(2885), 66368, "C55B43DD1A043081DDC0B13AF78D77C7C86071FCAEDE3111A3F57DDD4768542FF66A6B946CDB4DF6F865C180ED48145B46345D5F386F38C4A33F4B743607749D", new byte[] { 194, 226, 32, 217, 10, 153, 19, 109, 150, 255, 38, 237, 233, 155, 147, 220, 203, 245, 158, 169, 41, 91, 223, 212, 106, 31, 1, 153, 119, 127, 158, 149 }, "e8c3efc5-2caf-4fc0-8713-b05f9d2609e0" });
        }
    }
}
