using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTypeTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppProductTypeTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppProductTypeTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppProductTypeTranslations_AppProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "AppProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppProductTypeTranslations_ProductTypeId_Language",
                table: "AppProductTypeTranslations",
                columns: new[] { "ProductTypeId", "Language" },
                unique: true);

            const string defaultLanguage = "en";

            migrationBuilder.Sql($@"
                INSERT INTO ""AppProductTypeTranslations"" (""Id"", ""ProductTypeId"", ""Language"", ""Name"")
                SELECT pt.""Id"", pt.""Id"", '{defaultLanguage}', pt.""Name""
                FROM ""AppProductTypes"" pt
                WHERE pt.""Name"" IS NOT NULL
                  AND btrim(pt.""Name"") <> ''
                  AND NOT EXISTS (
                      SELECT 1
                      FROM ""AppProductTypeTranslations"" ptt
                      WHERE ptt.""ProductTypeId"" = pt.""Id""
                        AND ptt.""Language"" = '{defaultLanguage}'
                  );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppProductTypeTranslations");
        }
    }
}
