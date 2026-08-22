using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogEntityTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppBrandModelTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBrandModelTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBrandModelTranslations_AppBrandModels_BrandModelId",
                        column: x => x.BrandModelId,
                        principalTable: "AppBrandModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppBrandTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBrandTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBrandTranslations_AppBrands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "AppBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppCategoryTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCategoryTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCategoryTranslations_AppCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "AppCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppProductTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppProductTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppProductTranslations_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppBrandModelTranslations_BrandModelId_Language",
                table: "AppBrandModelTranslations",
                columns: new[] { "BrandModelId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppBrandTranslations_BrandId_Language",
                table: "AppBrandTranslations",
                columns: new[] { "BrandId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCategoryTranslations_CategoryId_Language",
                table: "AppCategoryTranslations",
                columns: new[] { "CategoryId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProductTranslations_ProductId_Language",
                table: "AppProductTranslations",
                columns: new[] { "ProductId", "Language" },
                unique: true);

            const string defaultLanguage = "en";

            // Backfill translations from legacy root Name/Description fields.
            migrationBuilder.Sql($@"
                INSERT INTO ""AppCategoryTranslations"" (""Id"", ""CategoryId"", ""Language"", ""Name"")
                SELECT c.""Id"", c.""Id"", '{defaultLanguage}', c.""Name""
                FROM ""AppCategories"" c
                WHERE c.""Name"" IS NOT NULL
                  AND btrim(c.""Name"") <> ''
                  AND NOT EXISTS (
                      SELECT 1
                      FROM ""AppCategoryTranslations"" ct
                      WHERE ct.""CategoryId"" = c.""Id""
                        AND ct.""Language"" = '{defaultLanguage}'
                  );");

            migrationBuilder.Sql($@"
                INSERT INTO ""AppBrandTranslations"" (""Id"", ""BrandId"", ""Language"", ""Name"", ""Description"")
                SELECT b.""Id"", b.""Id"", '{defaultLanguage}', b.""Name"", b.""Description""
                FROM ""AppBrands"" b
                WHERE b.""Name"" IS NOT NULL
                  AND btrim(b.""Name"") <> ''
                  AND NOT EXISTS (
                      SELECT 1
                      FROM ""AppBrandTranslations"" bt
                      WHERE bt.""BrandId"" = b.""Id""
                        AND bt.""Language"" = '{defaultLanguage}'
                  );");

            migrationBuilder.Sql($@"
                INSERT INTO ""AppBrandModelTranslations"" (""Id"", ""BrandModelId"", ""Language"", ""Name"")
                SELECT bm.""Id"", bm.""Id"", '{defaultLanguage}', bm.""Name""
                FROM ""AppBrandModels"" bm
                WHERE bm.""Name"" IS NOT NULL
                  AND btrim(bm.""Name"") <> ''
                  AND NOT EXISTS (
                      SELECT 1
                      FROM ""AppBrandModelTranslations"" bmt
                      WHERE bmt.""BrandModelId"" = bm.""Id""
                        AND bmt.""Language"" = '{defaultLanguage}'
                  );");

            migrationBuilder.Sql($@"
                INSERT INTO ""AppProductTranslations"" (""Id"", ""ProductId"", ""Language"", ""Name"", ""Description"")
                SELECT p.""Id"", p.""Id"", '{defaultLanguage}', p.""Name"", p.""Description""
                FROM ""AppProducts"" p
                WHERE p.""Name"" IS NOT NULL
                  AND btrim(p.""Name"") <> ''
                  AND NOT EXISTS (
                      SELECT 1
                      FROM ""AppProductTranslations"" pt
                      WHERE pt.""ProductId"" = p.""Id""
                        AND pt.""Language"" = '{defaultLanguage}'
                  );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppBrandModelTranslations");

            migrationBuilder.DropTable(
                name: "AppBrandTranslations");

            migrationBuilder.DropTable(
                name: "AppCategoryTranslations");

            migrationBuilder.DropTable(
                name: "AppProductTranslations");
        }
    }
}
