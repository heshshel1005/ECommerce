using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributeOptionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppAttributeOptionTranslations_AttributeOptionId_Language",
                table: "AppAttributeOptionTranslations");

            migrationBuilder.CreateTable(
                name: "AppAttributeOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    AttributeDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAttributeOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppAttributeOptions_AppAttributeDefinitions_AttributeDefini~",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AppAttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeOptionTranslations_AttributeOptionId",
                table: "AppAttributeOptionTranslations",
                column: "AttributeOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeOptionTranslations_TenantId_AttributeOptionId_L~",
                table: "AppAttributeOptionTranslations",
                columns: new[] { "TenantId", "AttributeOptionId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeOptions_AttributeDefinitionId",
                table: "AppAttributeOptions",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeOptions_TenantId_AttributeDefinitionId_Value",
                table: "AppAttributeOptions",
                columns: new[] { "TenantId", "AttributeDefinitionId", "Value" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppAttributeOptionTranslations_AppAttributeOptions_Attribut~",
                table: "AppAttributeOptionTranslations",
                column: "AttributeOptionId",
                principalTable: "AppAttributeOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAttributeOptionTranslations_AppAttributeOptions_Attribut~",
                table: "AppAttributeOptionTranslations");

            migrationBuilder.DropTable(
                name: "AppAttributeOptions");

            migrationBuilder.DropIndex(
                name: "IX_AppAttributeOptionTranslations_AttributeOptionId",
                table: "AppAttributeOptionTranslations");

            migrationBuilder.DropIndex(
                name: "IX_AppAttributeOptionTranslations_TenantId_AttributeOptionId_L~",
                table: "AppAttributeOptionTranslations");

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeOptionTranslations_AttributeOptionId_Language",
                table: "AppAttributeOptionTranslations",
                columns: new[] { "AttributeOptionId", "Language" },
                unique: true);
        }
    }
}
