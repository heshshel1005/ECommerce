using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ContactEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ContactPhone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ContactName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ShippingStreet = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ShippingStreet2 = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ShippingCity = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ShippingRegion = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ShippingPostalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ShippingCountry = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ShippingInstructions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BillingSameAsShipping = table.Column<bool>(type: "boolean", nullable: false),
                    BillingStreet = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    BillingStreet2 = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    BillingCity = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BillingRegion = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BillingPostalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    BillingCountry = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ShippingMethodCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ShippingMethodName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ShippingAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Sku = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    OrderId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppOrderLines_AppOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "AppOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppOrderLines_AppOrders_OrderId1",
                        column: x => x.OrderId1,
                        principalTable: "AppOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderLines_OrderId",
                table: "AppOrderLines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderLines_OrderId1",
                table: "AppOrderLines",
                column: "OrderId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_Status",
                table: "AppOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_UserId",
                table: "AppOrders",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppOrderLines");

            migrationBuilder.DropTable(
                name: "AppOrders");
        }
    }
}
