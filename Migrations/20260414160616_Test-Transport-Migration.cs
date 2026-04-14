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
            migrationBuilder.DropColumn(
                name: "Transport",
                table: "RouteSegments");

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
                name: "RouteSegmentTransport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RouteSegmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransoprtId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteSegmentTransport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteSegmentTransport_RouteSegments_RouteSegmentId",
                        column: x => x.RouteSegmentId,
                        principalTable: "RouteSegments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteSegmentTransport_Transports_TransoprtId",
                        column: x => x.TransoprtId,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteSegmentTransport_RouteSegmentId_TransoprtId",
                table: "RouteSegmentTransport",
                columns: new[] { "RouteSegmentId", "TransoprtId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteSegmentTransport_TransoprtId",
                table: "RouteSegmentTransport",
                column: "TransoprtId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteSegmentTransport");

            migrationBuilder.DropTable(
                name: "Transports");

            migrationBuilder.AddColumn<int>(
                name: "Transport",
                table: "RouteSegments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
