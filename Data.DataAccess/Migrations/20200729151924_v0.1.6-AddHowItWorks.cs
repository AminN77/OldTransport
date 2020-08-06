using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class v016AddHowItWorks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HowItWorks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    SettingsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HowItWorks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HowItWorks_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 29, 19, 49, 24, 182, DateTimeKind.Local).AddTicks(4154), 6247, "01B25371C55AFC90F3C319DE555EDD7AE1B4710745847C8AC26864A04705F9E99885E49CFD55E9031F9C8E2622E6C096F92FCEFFFAF7A1533AD1FAC250E6AAE3", new byte[] { 108, 229, 126, 172, 231, 64, 100, 227, 191, 139, 126, 89, 148, 15, 221, 178, 204, 85, 41, 147, 149, 103, 90, 122, 133, 106, 199, 9, 9, 154, 60, 40 }, "c0455ea8-688a-4803-8adf-253a18c8865e" });

            migrationBuilder.CreateIndex(
                name: "IX_HowItWorks_SettingsId",
                table: "HowItWorks",
                column: "SettingsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HowItWorks");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 29, 19, 43, 2, 735, DateTimeKind.Local).AddTicks(141), 57621, "F53B0B674F7CB585AA4ECED8BC9218A2CA783F0293D61F57CAB7F33BCEF9B6AAE24EAD492C1DFB3053CE4B2C71BC375873F4A8C143F6AA2CDB04ED9744F63F52", new byte[] { 158, 217, 218, 99, 36, 139, 169, 40, 243, 101, 184, 15, 54, 248, 103, 78, 10, 41, 59, 10, 243, 240, 165, 217, 55, 151, 208, 220, 45, 144, 200, 67 }, "f7062cbe-018c-4f1a-ab6b-c10292e7d76b" });
        }
    }
}
