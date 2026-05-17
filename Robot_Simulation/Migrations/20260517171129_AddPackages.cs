using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Robot_Simulation.Migrations
{
    /// <inheritdoc />
    public partial class AddPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    StorageTime = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    BatteryCost = table.Column<float>(type: "real", nullable: false),
                    WareHouseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Packages_WareHouses_WareHouseId",
                        column: x => x.WareHouseId,
                        principalTable: "WareHouses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Packages_WareHouseId",
                table: "Packages",
                column: "WareHouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Packages");
        }
    }
}
