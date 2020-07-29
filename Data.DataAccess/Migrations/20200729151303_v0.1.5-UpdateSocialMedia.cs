using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class v015UpdateSocialMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HowItWorks",
                table: "Settings");

            migrationBuilder.AddColumn<int>(
                name: "SettingsId",
                table: "SocialMedias",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ContactUs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailAddress = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    CreateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailAddress = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 29, 19, 43, 2, 735, DateTimeKind.Local).AddTicks(141), 57621, "F53B0B674F7CB585AA4ECED8BC9218A2CA783F0293D61F57CAB7F33BCEF9B6AAE24EAD492C1DFB3053CE4B2C71BC375873F4A8C143F6AA2CDB04ED9744F63F52", new byte[] { 158, 217, 218, 99, 36, 139, 169, 40, 243, 101, 184, 15, 54, 248, 103, 78, 10, 41, 59, 10, 243, 240, 165, 217, 55, 151, 208, 220, 45, 144, 200, 67 }, "f7062cbe-018c-4f1a-ab6b-c10292e7d76b" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialMedias_SettingsId",
                table: "SocialMedias",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SocialMedias_Settings_SettingsId",
                table: "SocialMedias",
                column: "SettingsId",
                principalTable: "Settings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SocialMedias_Settings_SettingsId",
                table: "SocialMedias");

            migrationBuilder.DropTable(
                name: "ContactUs");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_SocialMedias_SettingsId",
                table: "SocialMedias");

            migrationBuilder.DropColumn(
                name: "SettingsId",
                table: "SocialMedias");

            migrationBuilder.AddColumn<string>(
                name: "HowItWorks",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 7, 16, 19, 11, 47, 783, DateTimeKind.Local).AddTicks(3336), 91988, "C0ACA888360B30437BB63F9B68494D5D673D9755D2C28255502E6610C486523CFED853AB26DED1CB4BE37E475ABC320BF3FB8BC08040E3939E17FE21635B41B4", new byte[] { 43, 79, 177, 210, 94, 236, 60, 144, 161, 30, 54, 185, 224, 84, 207, 58, 88, 52, 139, 106, 154, 167, 106, 102, 145, 34, 236, 219, 185, 135, 20, 133 }, "9618146f-e855-431b-a09a-5d344f5baac8" });
        }
    }
}
