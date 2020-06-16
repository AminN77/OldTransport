using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V08CitiesAndCountries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 16, 12, 3, 34, 846, DateTimeKind.Local).AddTicks(3184), 82416, "A3794BD92C0ED2DB0C6B4CBCBC14AAFAA8C92C99BD71E58E577F39AA04BCEE328E2FC2722F7ED332B0CDD5BB4CF248F2BF8A495EBA728E7891EBDA35166C4B74", new byte[] { 13, 141, 160, 138, 248, 93, 90, 101, 18, 7, 152, 13, 131, 71, 112, 125, 3, 111, 12, 87, 54, 165, 248, 126, 47, 208, 141, 235, 236, 31, 254, 148 }, "588b17fe-1c74-455b-8902-e497554a84de" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 16, 11, 32, 16, 526, DateTimeKind.Local).AddTicks(6822), 87060, "92C179A686EED816074DBB64608E9ECE1B4C385879FA213374837468BBCB5A711815C5E9EE2E244815A79B4BF3547F528FF2B4236786641A546C4576BEE63262", new byte[] { 3, 81, 20, 169, 221, 217, 201, 82, 104, 141, 246, 187, 111, 49, 46, 200, 1, 51, 109, 164, 4, 237, 181, 30, 42, 10, 248, 40, 15, 162, 144, 6 }, "29ec0518-44cd-4258-a869-ccb5f74f1552" });
        }
    }
}
