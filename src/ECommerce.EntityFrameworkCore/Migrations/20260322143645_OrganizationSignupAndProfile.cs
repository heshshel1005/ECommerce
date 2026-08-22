using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class OrganizationSignupAndProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppOrganizationProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    LegalName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BusinessType = table.Column<int>(type: "integer", nullable: false),
                    Website = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ShortDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    LogoFilePathOrKey = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppOrganizationProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppOrganizationSignupRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    LegalName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BusinessType = table.Column<int>(type: "integer", nullable: false),
                    Website = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ShortDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    LogoFilePathOrKey = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    AdminEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AdminUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AdminDisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AdminPasswordCipher = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RejectionReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ReviewedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReviewerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedTenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppOrganizationSignupRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppOrganizationProfiles_TenantId",
                table: "AppOrganizationProfiles",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrganizationSignupRequests_CreatedTenantId",
                table: "AppOrganizationSignupRequests",
                column: "CreatedTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrganizationSignupRequests_Status",
                table: "AppOrganizationSignupRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrganizationSignupRequests_TenantName",
                table: "AppOrganizationSignupRequests",
                column: "TenantName",
                unique: true,
                filter: "\"Status\" = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppOrganizationProfiles");

            migrationBuilder.DropTable(
                name: "AppOrganizationSignupRequests");
        }
    }
}
