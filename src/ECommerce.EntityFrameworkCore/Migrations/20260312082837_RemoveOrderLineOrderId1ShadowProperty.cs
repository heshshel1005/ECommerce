using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrderLineOrderId1ShadowProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrderLines_AppOrders_OrderId1",
                table: "AppOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_AppOrderLines_OrderId1",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "AppOrderLines");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId1",
                table: "AppOrderLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderLines_OrderId1",
                table: "AppOrderLines",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrderLines_AppOrders_OrderId1",
                table: "AppOrderLines",
                column: "OrderId1",
                principalTable: "AppOrders",
                principalColumn: "Id");
        }
    }
}
