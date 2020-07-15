using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V14UpdateAccept : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "Accepts",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 15, 16, 35, 22, 398, DateTimeKind.Local).AddTicks(6327), 4576, "38764993846F1AD1775EFC54DB12B94FF0FB33FF9188D070791AAF321DD693EFD58DBC2FEE17074A7EFAC64821DD6661AD9C0E3F21A018FB63B02AD718A765BE", new byte[] { 110, 98, 10, 172, 25, 154, 177, 84, 245, 142, 244, 234, 155, 160, 174, 2, 189, 218, 228, 183, 231, 204, 113, 165, 87, 162, 64, 80, 241, 92, 38, 146 }, "5e97e03b-09d0-44b5-a9f6-81c25e28b4a9" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Accepts");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 15, 16, 22, 18, 422, DateTimeKind.Local).AddTicks(4616), 9906, "48F7345D30841E25893D5592E0FE762214EA5E39B408C32AA8E8CAA4378DF37D539EEDCF73DCA5B1920C57C8A7BC297366CC03441361DA6032672FA7FDE27FCE", new byte[] { 135, 158, 181, 7, 97, 203, 98, 135, 169, 153, 136, 178, 52, 113, 199, 106, 209, 39, 246, 53, 136, 188, 197, 220, 98, 8, 143, 131, 25, 185, 249, 93 }, "f52d5586-e074-4204-a04d-eaccb71cd70a" });
        }
    }
}
