using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class CatalogCategoryTenantSlugUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppCategories_Slug",
                table: "AppCategories");

            migrationBuilder.CreateIndex(
                name: "IX_AppCategories_TenantId_Slug",
                table: "AppCategories",
                columns: new[] { "TenantId", "Slug" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppCategories_TenantId_Slug",
                table: "AppCategories");

            migrationBuilder.CreateIndex(
                name: "IX_AppCategories_Slug",
                table: "AppCategories",
                column: "Slug");
        }
    }
}
