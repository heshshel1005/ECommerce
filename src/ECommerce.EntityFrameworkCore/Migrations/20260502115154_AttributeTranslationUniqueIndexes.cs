using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AttributeTranslationUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppAttributeOptionTranslations_TenantId_AttributeOptionId_L~",
                table: "AppAttributeOptionTranslations");

            migrationBuilder.DropIndex(
                name: "IX_AppAttributeDefinitionTranslations_AttributeDefinitionId",
                table: "AppAttributeDefinitionTranslations");

            migrationBuilder.DropIndex(
                name: "IX_AppAttributeDefinitionTranslations_TenantId_AttributeDefini~",
                table: "AppAttributeDefinitionTranslations");

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeOptionTranslations_AttributeOptionId_Language",
                table: "AppAttributeOptionTranslations",
                columns: new[] { "AttributeOptionId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeDefinitionTranslations_AttributeDefinitionId_La~",
                table: "AppAttributeDefinitionTranslations",
                columns: new[] { "AttributeDefinitionId", "Language" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppAttributeOptionTranslations_AttributeOptionId_Language",
                table: "AppAttributeOptionTranslations");

            migrationBuilder.DropIndex(
                name: "IX_AppAttributeDefinitionTranslations_AttributeDefinitionId_La~",
                table: "AppAttributeDefinitionTranslations");

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeOptionTranslations_TenantId_AttributeOptionId_L~",
                table: "AppAttributeOptionTranslations",
                columns: new[] { "TenantId", "AttributeOptionId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeDefinitionTranslations_AttributeDefinitionId",
                table: "AppAttributeDefinitionTranslations",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeDefinitionTranslations_TenantId_AttributeDefini~",
                table: "AppAttributeDefinitionTranslations",
                columns: new[] { "TenantId", "AttributeDefinitionId", "Language" },
                unique: true);
        }
    }
}
