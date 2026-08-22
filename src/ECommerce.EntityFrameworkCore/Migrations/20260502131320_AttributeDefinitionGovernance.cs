using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AttributeDefinitionGovernance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GovernanceStatus",
                table: "AppAttributeDefinitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PublishedVersion",
                table: "AppAttributeDefinitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                UPDATE "AppAttributeDefinitions"
                SET "GovernanceStatus" = 2, "PublishedVersion" = 1
                WHERE "GovernanceStatus" = 0 AND "PublishedVersion" = 0;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GovernanceStatus",
                table: "AppAttributeDefinitions");

            migrationBuilder.DropColumn(
                name: "PublishedVersion",
                table: "AppAttributeDefinitions");
        }
    }
}
