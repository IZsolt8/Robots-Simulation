using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Robot_Simulation.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageCreatedOnDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedOnDay",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnDay",
                table: "Packages");
        }
    }
}
