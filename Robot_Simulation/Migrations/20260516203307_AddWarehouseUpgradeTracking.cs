using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Robot_Simulation.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseUpgradeTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Robots_WareHouses_WareHouseId",
                table: "Robots");

            migrationBuilder.CreateTable(
                name: "WarehouseUpgradePurchases",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WareHouseId = table.Column<int>(type: "int", nullable: false),
                    UpgradeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseUpgradePurchases", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WarehouseUpgradePurchases_WareHouses_WareHouseId",
                        column: x => x.WareHouseId,
                        principalTable: "WareHouses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseUpgradePurchases_WareHouseId",
                table: "WarehouseUpgradePurchases",
                column: "WareHouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Robots_WareHouses_WareHouseId",
                table: "Robots",
                column: "WareHouseId",
                principalTable: "WareHouses",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Robots_WareHouses_WareHouseId",
                table: "Robots");

            migrationBuilder.DropTable(
                name: "WarehouseUpgradePurchases");

            migrationBuilder.AddForeignKey(
                name: "FK_Robots_WareHouses_WareHouseId",
                table: "Robots",
                column: "WareHouseId",
                principalTable: "WareHouses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
