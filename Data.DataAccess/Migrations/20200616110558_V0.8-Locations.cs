using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V08Locations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Settings",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Settings",
                table: "Settings",
                column: "Id");

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

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "AboutUs", "ContactEmail", "ContactNumber", "Logo" },
                values: new object[] { 1, "We're The Transport Team", "abolfazl.sh1374@gmail.com", "+98 937 733 9223", "abcd" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 16, 15, 35, 56, 977, DateTimeKind.Local).AddTicks(5636), 84929, "1B3A5EB00E11506A015FFE16F2CC1CBC0D26D47EAE6E333B9BB51240FF03D503A8B38AFE22E28BD27E7B660F63538A2068090A57E11C977722D8F611BEAD26C4", new byte[] { 220, 45, 236, 69, 99, 182, 13, 211, 23, 206, 195, 244, 220, 40, 82, 166, 215, 126, 213, 91, 0, 212, 189, 24, 206, 175, 172, 35, 154, 217, 113, 120 }, "b3a6906e-4806-4eea-b96c-a776d6165446" });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Settings",
                table: "Settings");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Settings");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 16, 11, 32, 16, 526, DateTimeKind.Local).AddTicks(6822), 87060, "92C179A686EED816074DBB64608E9ECE1B4C385879FA213374837468BBCB5A711815C5E9EE2E244815A79B4BF3547F528FF2B4236786641A546C4576BEE63262", new byte[] { 3, 81, 20, 169, 221, 217, 201, 82, 104, 141, 246, 187, 111, 49, 46, 200, 1, 51, 109, 164, 4, 237, 181, 30, 42, 10, 248, 40, 15, 162, 144, 6 }, "29ec0518-44cd-4258-a869-ccb5f74f1552" });
        }
    }
}
