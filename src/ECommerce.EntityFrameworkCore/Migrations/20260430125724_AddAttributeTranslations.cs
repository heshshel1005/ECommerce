using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributeTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppAttributeDefinitionTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    AttributeDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAttributeDefinitionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppAttributeDefinitionTranslations_AppAttributeDefinitions_~",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AppAttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppAttributeOptionTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    AttributeOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAttributeOptionTranslations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeDefinitionTranslations_AttributeDefinitionId",
                table: "AppAttributeDefinitionTranslations",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeDefinitionTranslations_TenantId_AttributeDefini~",
                table: "AppAttributeDefinitionTranslations",
                columns: new[] { "TenantId", "AttributeDefinitionId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeOptionTranslations_TenantId_AttributeOptionId_L~",
                table: "AppAttributeOptionTranslations",
                columns: new[] { "TenantId", "AttributeOptionId", "Language" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppAttributeDefinitionTranslations");

            migrationBuilder.DropTable(
                name: "AppAttributeOptionTranslations");
        }
    }
}
