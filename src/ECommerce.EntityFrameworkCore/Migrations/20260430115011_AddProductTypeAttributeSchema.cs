using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTypeAttributeSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DynamicAttributesJson",
                table: "AppProducts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductTypeId",
                table: "AppProducts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppAttributeDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DataType = table.Column<int>(type: "integer", nullable: false),
                    AllowedValuesJson = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RegexPattern = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    MinValue = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxValue = table.Column<decimal>(type: "numeric", nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    IsRecommended = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAttributeDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppProductTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
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
                    table.PrimaryKey("PK_AppProductTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppProductTypeAttributeRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    ConditionalAttributeKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ConditionalOperator = table.Column<int>(type: "integer", nullable: true),
                    ConditionalExpectedValue = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppProductTypeAttributeRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppProductTypeAttributeRules_AppAttributeDefinitions_Attrib~",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AppAttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppProductTypeAttributeRules_AppProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "AppProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAttributeDefinitions_TenantId_Key",
                table: "AppAttributeDefinitions",
                columns: new[] { "TenantId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProductTypeAttributeRules_AttributeDefinitionId",
                table: "AppProductTypeAttributeRules",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProductTypeAttributeRules_ProductTypeId",
                table: "AppProductTypeAttributeRules",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProductTypeAttributeRules_TenantId_ProductTypeId_Attribu~",
                table: "AppProductTypeAttributeRules",
                columns: new[] { "TenantId", "ProductTypeId", "AttributeDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProductTypes_TenantId_Code",
                table: "AppProductTypes",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProductTypes_TenantId_Name",
                table: "AppProductTypes",
                columns: new[] { "TenantId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppProductTypeAttributeRules");

            migrationBuilder.DropTable(
                name: "AppAttributeDefinitions");

            migrationBuilder.DropTable(
                name: "AppProductTypes");

            migrationBuilder.DropColumn(
                name: "DynamicAttributesJson",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "AppProducts");
        }
    }
}
