using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrudPark.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueActiveLicensePlateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Memberships_LicensePlate",
                table: "Memberships",
                column: "LicensePlate",
                unique: true,
                filter: "\"IsActive\" = TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Memberships_LicensePlate",
                table: "Memberships");
        }
    }
}
