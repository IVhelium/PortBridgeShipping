using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortBridgeShipping.Migrations
{
    /// <inheritdoc />
    public partial class FixedMigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Containers_Routes_RouteId",
                table: "Containers");

            migrationBuilder.DropForeignKey(
                name: "FK_Containers_Statuses_StatusId",
                table: "Containers");

            migrationBuilder.DropIndex(
                name: "IX_RouteSegments_RouteId",
                table: "RouteSegments");

            migrationBuilder.CreateIndex(
                name: "IX_RouteSegments_RouteId_Order",
                table: "RouteSegments",
                columns: new[] { "RouteId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ContainerNumber",
                table: "Containers",
                column: "ContainerNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Containers_Routes_RouteId",
                table: "Containers",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Containers_Statuses_StatusId",
                table: "Containers",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Containers_Routes_RouteId",
                table: "Containers");

            migrationBuilder.DropForeignKey(
                name: "FK_Containers_Statuses_StatusId",
                table: "Containers");

            migrationBuilder.DropIndex(
                name: "IX_RouteSegments_RouteId_Order",
                table: "RouteSegments");

            migrationBuilder.DropIndex(
                name: "IX_Containers_ContainerNumber",
                table: "Containers");

            migrationBuilder.CreateIndex(
                name: "IX_RouteSegments_RouteId",
                table: "RouteSegments",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Containers_Routes_RouteId",
                table: "Containers",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Containers_Statuses_StatusId",
                table: "Containers",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
