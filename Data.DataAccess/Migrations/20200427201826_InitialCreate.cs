using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(maxLength: 32, nullable: false),
                    LastName = table.Column<string>(maxLength: 64, nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(type: "char(128)", maxLength: 128, nullable: false),
                    EmailAddress = table.Column<string>(type: "char(128)", maxLength: 128, nullable: false),
                    Picture = table.Column<string>(maxLength: 128, nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Salt = table.Column<byte[]>(maxLength: 32, nullable: false),
                    IterationCount = table.Column<int>(nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreateDateTime", "EmailAddress", "FirstName", "IsDeleted", "IsEnabled", "IterationCount", "LastName", "Password", "Picture", "Salt", "Username" },
                values: new object[] { 1, new DateTime(2020, 4, 28, 0, 48, 26, 191, DateTimeKind.Local).AddTicks(7840), "abolfazl.sh1374@gmail.com", "شرکت", false, true, 33271, "کندو", "D00F0CE1E55CB8D22BE3FAA47822FB12FDB61EE22ADFE21697C975EF8945D631C8A75E5150C6F090169F5EDB733EB978EF5FB28786609EAA93D03EF418A37888", "Source/1.jpg", new byte[] { 60, 61, 31, 75, 173, 200, 151, 55, 188, 197, 90, 4, 176, 168, 164, 238, 184, 44, 250, 156, 162, 107, 64, 44, 236, 91, 75, 167, 19, 75, 116, 138 }, "developersupport" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailAddress",
                table: "Users",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirstName",
                table: "Users",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastName",
                table: "Users",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
