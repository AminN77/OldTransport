using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.DataAccess.Migrations
{
    public partial class V06OfferAndAccept : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Projects");

            migrationBuilder.AlterColumn<int>(
                name: "Budget",
                table: "Projects",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Projects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransporterId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<int>(nullable: false),
                    EstimatedTime = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Offers_Transporters_TransporterId",
                        column: x => x.TransporterId,
                        principalTable: "Transporters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accepts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantId = table.Column<int>(nullable: false),
                    OfferId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accepts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accepts_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accepts_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 15, 13, 48, 2, 715, DateTimeKind.Local).AddTicks(5299), 76838, "EF640715E3D362F6D99BB4AF9A5F0D4C3CB836DAD1EA3E449EC549D8350E8F61DC39FB7C94077E8E7E63AA317637EB41B24F2DD770D03E80500171247C395263", new byte[] { 86, 165, 23, 64, 243, 240, 176, 146, 15, 225, 131, 208, 155, 59, 249, 129, 61, 216, 165, 130, 232, 71, 211, 208, 169, 58, 200, 73, 197, 116, 26, 169 }, "98289034-38b7-4921-a303-4a056be73756" });

            migrationBuilder.CreateIndex(
                name: "IX_Accepts_OfferId",
                table: "Accepts",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_Accepts_MerchantId_OfferId",
                table: "Accepts",
                columns: new[] { "MerchantId", "OfferId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ProjectId",
                table: "Offers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_TransporterId_ProjectId",
                table: "Offers",
                columns: new[] { "TransporterId", "ProjectId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accepts");

            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Projects");

            migrationBuilder.AlterColumn<double>(
                name: "Budget",
                table: "Projects",
                type: "float",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDateTime", "IterationCount", "Password", "Salt", "SerialNumber" },
                values: new object[] { new DateTime(2020, 6, 14, 12, 17, 29, 943, DateTimeKind.Local).AddTicks(4831), 91312, "647DD12607A9B64930B741BCF2606EC30D464D3E15FB1270345726D504191E5CF6811BFE1E3E82770A5B8463537E513984A611686D14F84C5D0ED3142CE606A0", new byte[] { 196, 215, 143, 40, 77, 135, 54, 218, 173, 188, 30, 214, 211, 200, 170, 208, 105, 218, 110, 2, 32, 211, 204, 18, 194, 203, 0, 226, 141, 111, 205, 111 }, "596706c1-3f75-4722-9671-dde67b2252de" });
        }
    }
}
