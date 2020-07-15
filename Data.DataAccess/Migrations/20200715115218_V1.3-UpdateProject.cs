using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V13UpdateProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cargo",
                table: "Projects",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Dimention",
                table: "Projects",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "Projects",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 15, 16, 22, 18, 422, DateTimeKind.Local).AddTicks(4616), 9906, "48F7345D30841E25893D5592E0FE762214EA5E39B408C32AA8E8CAA4378DF37D539EEDCF73DCA5B1920C57C8A7BC297366CC03441361DA6032672FA7FDE27FCE", new byte[] { 135, 158, 181, 7, 97, 203, 98, 135, 169, 153, 136, 178, 52, 113, 199, 106, 209, 39, 246, 53, 136, 188, 197, 220, 98, 8, 143, 131, 25, 185, 249, 93 }, "f52d5586-e074-4204-a04d-eaccb71cd70a" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cargo",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Dimention",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 12, 22, 29, 43, 408, DateTimeKind.Local).AddTicks(7884), 83064, "262C4E4169FA263CC4184CDC9B0A358EF176B65B0EC662BDCE48FE196013571CE140552BEBCC09417DAD4C590D85B1ADFB9612A5F6E9A8536C1D016229297668", new byte[] { 189, 91, 78, 44, 230, 193, 180, 89, 74, 139, 182, 28, 45, 132, 128, 87, 201, 105, 218, 78, 131, 20, 189, 210, 106, 235, 247, 94, 137, 162, 205, 57 }, "9b8a53cf-7df1-46a7-9838-efe78366f198" });
        }
    }
}
