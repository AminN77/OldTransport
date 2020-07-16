using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V14UpdateProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Projects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 16, 19, 11, 47, 783, DateTimeKind.Local).AddTicks(3336), 91988, "C0ACA888360B30437BB63F9B68494D5D673D9755D2C28255502E6610C486523CFED853AB26DED1CB4BE37E475ABC320BF3FB8BC08040E3939E17FE21635B41B4", new byte[] { 43, 79, 177, 210, 94, 236, 60, 144, 161, 30, 54, 185, 224, 84, 207, 58, 88, 52, 139, 106, 154, 167, 106, 102, 145, 34, 236, 219, 185, 135, 20, 133 }, "9618146f-e855-431b-a09a-5d344f5baac8" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 15, 16, 35, 22, 398, DateTimeKind.Local).AddTicks(6327), 4576, "38764993846F1AD1775EFC54DB12B94FF0FB33FF9188D070791AAF321DD693EFD58DBC2FEE17074A7EFAC64821DD6661AD9C0E3F21A018FB63B02AD718A765BE", new byte[] { 110, 98, 10, 172, 25, 154, 177, 84, 245, 142, 244, 234, 155, 160, 174, 2, 189, 218, 228, 183, 231, 204, 113, 165, 87, 162, 64, 80, 241, 92, 38, 146 }, "5e97e03b-09d0-44b5-a9f6-81c25e28b4a9" });
        }
    }
}
