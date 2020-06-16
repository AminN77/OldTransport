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
                    ContactEmail = table.Column<string>(nullable: false),
                    AboutUs = table.Column<string>(nullable: false),
                    Logo = table.Column<string>(nullable: false),
                    ContactNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "SocialMedias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Link = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMedias", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 16, 11, 32, 16, 526, DateTimeKind.Local).AddTicks(6822), 87060, "92C179A686EED816074DBB64608E9ECE1B4C385879FA213374837468BBCB5A711815C5E9EE2E244815A79B4BF3547F528FF2B4236786641A546C4576BEE63262", new byte[] { 3, 81, 20, 169, 221, 217, 201, 82, 104, 141, 246, 187, 111, 49, 46, 200, 1, 51, 109, 164, 4, 237, 181, 30, 42, 10, 248, 40, 15, 162, 144, 6 }, "29ec0518-44cd-4258-a869-ccb5f74f1552" });
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
