using Microsoft.EntityFrameworkCore;
using Robot_Simulation.Data;
using Robot_Simulation.Models;
using Robots_Simulation.Controllers;
using System.Text.Json;
using Xunit;

namespace Robot_Simulation.Tests
{
    public class SimulationTests
    {
        private RobotSimulationContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<RobotSimulationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new RobotSimulationContext(options);
        }

        [Fact]
        public async Task Test_CreateGame()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new HomeController(context);
            var newGame = new Game { GameName = "Test Game" };

            // Act
            var result = await controller.Create(newGame);

            // Assert
            var savedGame = await context.Games.Include(g => g.WareHouse).FirstOrDefaultAsync(g => g.GameName == "Test Game");
            Assert.NotNull(savedGame);
            Assert.NotNull(savedGame.WareHouse);
            Assert.Equal(20000, savedGame.Balance);
        }

        [Fact]
        public async Task Test_DeleteGame()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var warehouse = new WareHouse();
            context.WareHouses.Add(warehouse);
            var game = new Game { GameName = "Game to Delete", WareHouse = warehouse };
            context.Games.Add(game);
            await context.SaveChangesAsync();

            var controller = new HomeController(context);

            // Act
            var result = await controller.DeleteConfirmed(game.ID);

            // Assert
            var deletedGame = await context.Games.FirstOrDefaultAsync(g => g.ID == game.ID);
            var deletedWarehouse = await context.WareHouses.FirstOrDefaultAsync(w => w.ID == warehouse.ID);
            Assert.Null(deletedGame);
            Assert.Null(deletedWarehouse);
        }

        [Fact]
        public async Task Test_LoadGame()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            
            var w1 = new WareHouse();
            var w2 = new WareHouse();
            context.WareHouses.Add(w1);
            context.WareHouses.Add(w2);
            await context.SaveChangesAsync();

            context.Games.Add(new Game { GameName = "Loadable 1", Balance = 20000, WarehouseId = w1.ID });
            context.Games.Add(new Game { GameName = "Loadable 2", Balance = 20000, WarehouseId = w2.ID });
            await context.SaveChangesAsync();

            var controller = new HomeController(context);

            // Act
            var result = await controller.LoadGame(null) as Microsoft.AspNetCore.Mvc.ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as List<Game>;
            Assert.NotNull(model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Test_BuyPackingRobot()
        {
            // Arrange
            var warehouse = new WareHouse();
            var shopData = JsonDocument.Parse("{\"MaintenanceFee\": 100, \"PackingSpeed\": 5, \"BatterySize\": 100}").RootElement;

            // Act
            var robot = warehouse.AddRobotFromShop("PackingRobot", "Packer Robi", shopData);

            // Assert
            Assert.Single(warehouse.Robots);
            Assert.IsType<PackingRobot>(robot);
            var pRobot = robot as PackingRobot;
            Assert.Equal(100, pRobot.MaintenanceFee);
            Assert.Equal(5, pRobot.PackingSpeed);
            Assert.Equal(100, pRobot.BatterySize);
        }

        [Fact]
        public void Test_BuyChargingRobot()
        {
            // Arrange
            var warehouse = new WareHouse();
            var shopData = JsonDocument.Parse("{\"MaintenanceFee\": 150, \"ChargingSpeed\": 10.5, \"MaxChargingCapacity\": 3}").RootElement;

            // Act
            var robot = warehouse.AddRobotFromShop("ChargingRobot", "Charger Robi", shopData);

            // Assert
            Assert.Single(warehouse.Robots);
            Assert.IsType<ChargingRobot>(robot);
            var cRobot = robot as ChargingRobot;
            Assert.Equal(150, cRobot.MaintenanceFee);
            Assert.Equal(10.5f, cRobot.ChargingSpeed);
            Assert.Equal(3, cRobot.MaxChargingCapacity);
        }

        [Fact]
        public void Test_WarehouseUpgrade()
        {
            // Arrange
            var warehouse = new WareHouse { StorgarSize = 10 };

            // Act
            warehouse.AddUpgrade("Upgrade 1", 5);

            // Assert
            Assert.Equal(15, warehouse.StorgarSize);
            Assert.Single(warehouse.UpgradesPurchased);
            Assert.Equal(1, warehouse.UpgradesPurchased.First().Quantity);
            Assert.Equal("Upgrade 1", warehouse.UpgradesPurchased.First().UpgradeName);

            // Act 2 - buy the same upgrade again
            warehouse.AddUpgrade("Upgrade 1", 5);
            Assert.Equal(20, warehouse.StorgarSize);
            Assert.Equal(2, warehouse.UpgradesPurchased.First().Quantity);
        }

        [Fact]
        public void Test_PackingLogic()
        {
            // Arrange
            var warehouse = new WareHouse();
            var packingShopData = JsonDocument.Parse("{\"MaintenanceFee\": 100, \"PackingSpeed\": 2, \"BatterySize\": 100}").RootElement;
            warehouse.AddRobotFromShop("PackingRobot", "Robi", packingShopData);

            var pkg1 = new Packages { Type = "nem romlandó", Status = false, BatteryCost = 10, CreatedOnDay = 0 };
            var pkg2 = new Packages { Type = "nem romlandó", Status = false, BatteryCost = 10, CreatedOnDay = 0 };
            var pkg3 = new Packages { Type = "nem romlandó", Status = false, BatteryCost = 10, CreatedOnDay = 0 };
            warehouse.Packages.Add(pkg1);
            warehouse.Packages.Add(pkg2);
            warehouse.Packages.Add(pkg3);

            // Act - packing per hour: the robot's PackingSpeed = 2
            warehouse.ProcessHourlyPacking(0);

            // Assert
            Assert.True(pkg1.Status); // Packed
            Assert.True(pkg2.Status); // Packed
            Assert.False(pkg3.Status); // The robot had no capacity left in the first hour
            Assert.Equal(80, warehouse.Robots.OfType<PackingRobot>().First().BatteryLevel); // 100 - (2 * 10)
        }

        [Fact]
        public void Test_ChargingLogic()
        {
            // Arrange
            var warehouse = new WareHouse();
            var packingShopData = JsonDocument.Parse("{\"MaintenanceFee\": 100, \"PackingSpeed\": 1, \"BatterySize\": 100}").RootElement;
            var chargingShopData = JsonDocument.Parse("{\"MaintenanceFee\": 150, \"ChargingSpeed\": 20.0, \"MaxChargingCapacity\": 1}").RootElement;
            
            warehouse.AddRobotFromShop("PackingRobot", "PackerRobi", packingShopData);
            warehouse.AddRobotFromShop("ChargingRobot", "ChargerRobi", chargingShopData);

            var pRobot = warehouse.Robots.OfType<PackingRobot>().First();
            pRobot.BatteryLevel = 0; // Fully depleted
            pRobot.IsCharging = true; // Waiting to charge

            // Act - one hour passes, the charging robot charges it
            warehouse.ProcessHourlyPacking(0);

            // Assert
            Assert.Equal(20, pRobot.BatteryLevel);
            // 0 + 20 charging speed = 20 battery level
        }
    }
}
