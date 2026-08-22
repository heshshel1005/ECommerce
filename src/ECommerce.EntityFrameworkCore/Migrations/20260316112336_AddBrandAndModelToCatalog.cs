using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandAndModelToCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BrandId",
                table: "AppProducts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId",
                table: "AppProducts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppBrands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Slug = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_AppBrands", x => x.Id);
                });

            // Seed a default brand and assign it to existing products so the non-nullable FK can be created safely.
            var defaultBrandId = new Guid("11111111-1111-1111-1111-111111111111");

            migrationBuilder.InsertData(
                table: "AppBrands",
                columns: new[]
                {
                    "Id",
                    "Name",
                    "Slug",
                    "Description",
                    "IsActive",
                    "ExtraProperties",
                    "ConcurrencyStamp",
                    "CreationTime",
                    "CreatorId",
                    "LastModificationTime",
                    "LastModifierId"
                },
                values: new object[]
                {
                    defaultBrandId,
                    "Default Brand",
                    "default-brand",
                    null,
                    true,
                    "{}",
                    Guid.NewGuid().ToString("N"),
                    DateTime.UtcNow,
                    null,
                    null,
                    null
                });

            migrationBuilder.Sql(
                $"UPDATE \"AppProducts\" SET \"BrandId\" = '{defaultBrandId}' WHERE \"BrandId\" = '00000000-0000-0000-0000-000000000000';");

            migrationBuilder.CreateTable(
                name: "AppBrandModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    table.PrimaryKey("PK_AppBrandModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBrandModels_AppBrands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "AppBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppProducts_BrandId",
                table: "AppProducts",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProducts_ModelId",
                table: "AppProducts",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBrandModels_BrandId",
                table: "AppBrandModels",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBrandModels_BrandId_Name",
                table: "AppBrandModels",
                columns: new[] { "BrandId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AppBrands_Name",
                table: "AppBrands",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_AppProducts_AppBrandModels_ModelId",
                table: "AppProducts",
                column: "ModelId",
                principalTable: "AppBrandModels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppProducts_AppBrands_BrandId",
                table: "AppProducts",
                column: "BrandId",
                principalTable: "AppBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppProducts_AppBrandModels_ModelId",
                table: "AppProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_AppProducts_AppBrands_BrandId",
                table: "AppProducts");

            migrationBuilder.DropTable(
                name: "AppBrandModels");

            migrationBuilder.DropTable(
                name: "AppBrands");

            migrationBuilder.DropIndex(
                name: "IX_AppProducts_BrandId",
                table: "AppProducts");

            migrationBuilder.DropIndex(
                name: "IX_AppProducts_ModelId",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "AppProducts");
        }
    }
}
