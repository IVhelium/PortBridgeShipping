using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortBridgeShipping.Migrations
{
    /// <inheritdoc />
    public partial class TestTransportMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Containers_Routes_RouteId",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "Transport",
                table: "RouteSegments");

            migrationBuilder.AlterColumn<int>(
                name: "RouteId",
                table: "Containers",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "Transports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransportNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TransportType = table.Column<int>(type: "INTEGER", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteSegmentTransports",
                columns: table => new
                {
                    RouteSegmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransportId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteSegmentTransports", x => new { x.RouteSegmentId, x.TransportId });
                    table.ForeignKey(
                        name: "FK_RouteSegmentTransports_RouteSegments_RouteSegmentId",
                        column: x => x.RouteSegmentId,
                        principalTable: "RouteSegments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteSegmentTransports_Transports_TransportId",
                        column: x => x.TransportId,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteSegmentTransports_TransportId",
                table: "RouteSegmentTransports",
                column: "TransportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Containers_Routes_RouteId",
                table: "Containers",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Containers_Routes_RouteId",
                table: "Containers");

            migrationBuilder.DropTable(
                name: "RouteSegmentTransports");

            migrationBuilder.DropTable(
                name: "Transports");

            migrationBuilder.AddColumn<int>(
                name: "Transport",
                table: "RouteSegments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "RouteId",
                table: "Containers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Containers_Routes_RouteId",
                table: "Containers",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
