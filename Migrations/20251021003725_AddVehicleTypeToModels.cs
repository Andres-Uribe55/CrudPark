using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrudPark.API.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleTypeToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VehicleType",
                table: "Tickets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleType",
                table: "Rates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Memberships",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleType",
                table: "Memberships",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "Memberships");
        }
    }
}
