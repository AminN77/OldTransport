using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V07Settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    ContactEmail = table.Column<string>(nullable: true),
                    AboutUs = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: true),
                    ContactNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "SocialMedias",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 16, 10, 55, 43, 153, DateTimeKind.Local).AddTicks(6922), 37779, "34A447FF8C91BCE0BA3F777FCC3EC1DB7497DCA389603151FC0769D0AC605A43533160C5CCA111A51C327AE7C0F25480942542FAAC377E3BA374DC496950E872", new byte[] { 100, 207, 220, 35, 3, 99, 98, 243, 231, 80, 103, 208, 91, 247, 114, 10, 7, 251, 128, 8, 59, 87, 46, 49, 234, 10, 7, 154, 136, 102, 133, 244 }, "6b9af8a8-255a-45b8-9888-5d7b5c208889" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "SocialMedias");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 15, 13, 48, 2, 715, DateTimeKind.Local).AddTicks(5299), 76838, "EF640715E3D362F6D99BB4AF9A5F0D4C3CB836DAD1EA3E449EC549D8350E8F61DC39FB7C94077E8E7E63AA317637EB41B24F2DD770D03E80500171247C395263", new byte[] { 86, 165, 23, 64, 243, 240, 176, 146, 15, 225, 131, 208, 155, 59, 249, 129, 61, 216, 165, 130, 232, 71, 211, 208, 169, 58, 200, 73, 197, 116, 26, 169 }, "98289034-38b7-4921-a303-4a056be73756" });
        }
    }
}
