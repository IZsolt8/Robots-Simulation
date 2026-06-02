using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Robot_Simulation.Migrations
{
    /// <inheritdoc />
    public partial class AddIsGameOver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGameOver",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGameOver",
                table: "Games");
        }
    }
}
