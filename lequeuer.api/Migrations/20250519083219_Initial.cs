using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lequeuer.api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "restaurants",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    available_seats = table.Column<int>(type: "int", nullable: false),
                    version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_restaurants", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: false),
                    seated_at = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    vacated_at = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    lead_guest_name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    restaurant_id = table.Column<int>(type: "int", nullable: false),
                    number_of_diners = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_at = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservations", x => x.id);
                    table.ForeignKey(
                        name: "fk_reservations_restaurants_restaurant_id",
                        column: x => x.restaurant_id,
                        principalTable: "restaurants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_reservations_created_at",
                table: "reservations",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_reservations_deleted_at",
                table: "reservations",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_reservations_restaurant_id",
                table: "reservations",
                column: "restaurant_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservations_status",
                table: "reservations",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_restaurants_deleted_at",
                table: "restaurants",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_restaurants_name",
                table: "restaurants",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "restaurants");
        }
    }
}
