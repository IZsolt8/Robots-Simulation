using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Robot_Simulation.Data;

namespace Robot_Simulation.Controllers
{
    public class GamesController : Controller
    {
        private readonly RobotSimulationContext _context;
        private readonly IWebHostEnvironment _env;
        public GamesController(RobotSimulationContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var game = await _context.Games
                .Include(g => g.WareHouse)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        public async Task<IActionResult> PackingShop(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var game = await _context.Games
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            var jsonPath = Path.Combine(_env.WebRootPath, "data", "shop.json");
            var jsonString = await System.IO.File.ReadAllTextAsync(jsonPath);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);
            var packingRobotElement = doc.RootElement.GetProperty("PackingRobot");
            var packingRobots = System.Text.Json.JsonSerializer.Deserialize<List<Robot_Simulation.Models.PackingRobot>>(packingRobotElement.GetRawText())
                ?? new List<Robot_Simulation.Models.PackingRobot>();

            ViewBag.PackingRobots = packingRobots;

            var ownedCounts = new Dictionary<string, int>();
            if (game.WareHouse?.Robots != null)
            {
                foreach (var robot in game.WareHouse.Robots)
                {
                    if (ownedCounts.ContainsKey(robot.Name))
                        ownedCounts[robot.Name]++;
                    else
                        ownedCounts[robot.Name] = 1;
                }
            }
            ViewBag.OwnedCounts = ownedCounts;

            return View(game);
        }

        public async Task<IActionResult> PackageList(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var game = await _context.Games
                .Include(g => g.WareHouse)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        public async Task<IActionResult> Upgrades(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var game = await _context.Games
                .Include(g => g.WareHouse)
                .ThenInclude(w => w.UpgradesPurchased)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            var jsonPath = Path.Combine(_env.WebRootPath, "data", "shop.json");
            var jsonString = await System.IO.File.ReadAllTextAsync(jsonPath);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);
            var warehouseCards = new List<Robot_Simulation.Models.ShopCardViewModel>();
            foreach (var item in doc.RootElement.GetProperty("Warehouse shop").EnumerateArray())
            {
                var upgradeName = item.GetProperty("Name").GetString() ?? "";
                var ownedCount = game.WareHouse?.UpgradesPurchased.FirstOrDefault(u => u.UpgradeName == upgradeName)?.Quantity ?? 0;

                warehouseCards.Add(new Robot_Simulation.Models.ShopCardViewModel
                {
                    Name = upgradeName,
                    Price = item.GetProperty("Price").GetInt32(),
                    Img = item.GetProperty("Img").GetString() ?? "",
                    OwnedCount = ownedCount,
                    RobotType = "Upgrade",
                    GameId = game.ID,
                    Stats = new List<string>
                    {
                        $"Raktárméret bővítés: +{item.GetProperty("Size").GetInt32()}"
                    }
                });
            }

            ViewBag.WarehouseUpgrades = warehouseCards;

            return View(game);
        }

        public async Task<IActionResult> ChargingShop(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var game = await _context.Games
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            var jsonPath = Path.Combine(_env.WebRootPath, "data", "shop.json");
            var jsonString = await System.IO.File.ReadAllTextAsync(jsonPath);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);
            var chargingElement = doc.RootElement.GetProperty("ChargingRobot");
            var chargingRobots = System.Text.Json.JsonSerializer.Deserialize<List<Robot_Simulation.Models.ChargingRobot>>(chargingElement.GetRawText())
                ?? new List<Robot_Simulation.Models.ChargingRobot>();

            ViewBag.ChargingRobots = chargingRobots;

            var ownedCounts = new Dictionary<string, int>();
            if (game.WareHouse?.Robots != null)
            {
                foreach (var robot in game.WareHouse.Robots)
                {
                    if (ownedCounts.ContainsKey(robot.Name))
                        ownedCounts[robot.Name]++;
                    else
                        ownedCounts[robot.Name] = 1;
                }
            }
            ViewBag.OwnedCounts = ownedCounts;

            return View(game);
        }

        [HttpPost]
        public async Task<IActionResult> BuyRobot(int gameId, string robotType, string robotName)
        {
            var game = await _context.Games
                .Include(g => g.WareHouse)
                .FirstOrDefaultAsync(m => m.ID == gameId);

            if (game == null || game.WareHouse == null)
            {
                return Json(new { success = false, message = "A játék nem található." });
            }

            var jsonPath = Path.Combine(_env.WebRootPath, "data", "shop.json");
            var jsonString = await System.IO.File.ReadAllTextAsync(jsonPath);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);

            System.Text.Json.JsonElement? foundRobot = null;
            foreach (var item in doc.RootElement.GetProperty(robotType).EnumerateArray())
            {
                if (item.GetProperty("Name").GetString() == robotName)
                {
                    foundRobot = item;
                    break;
                }
            }

            if (foundRobot == null)
            {
                return Json(new { success = false, message = "A robot nem található." });
            }

            var price = foundRobot.Value.GetProperty("Price").GetInt32();

            if (game.Balance < price)
            {
                return Json(new { success = false, message = "Nincs elegendő egyenlege" });
            }

            game.Balance -= price;

            Robot_Simulation.Models.Robot newRobot;
            if (robotType == "PackingRobot")
            {
                newRobot = new Robot_Simulation.Models.PackingRobot
                {
                    Name = robotName,
                    MaintenanceFee = foundRobot.Value.GetProperty("MaintenanceFee").GetInt32(),
                    PackingSpeed = foundRobot.Value.GetProperty("PackingSpeed").GetInt32(),
                    BatteryLevel = foundRobot.Value.GetProperty("BatterySize").GetInt32(),
                    Status = true,
                    WareHouseId = game.WarehouseId
                };
            }
            else
            {
                newRobot = new Robot_Simulation.Models.ChargingRobot
                {
                    Name = robotName,
                    MaintenanceFee = foundRobot.Value.GetProperty("MaintenanceFee").GetInt32(),
                    ChargingSpeed = (float)foundRobot.Value.GetProperty("ChargingSpeed").GetDouble(),
                    Status = true,
                    WareHouseId = game.WarehouseId
                };
            }

            _context.Robots.Add(newRobot);
            await _context.SaveChangesAsync();

            return Json(new { success = true, balance = game.Balance });
        }

        [HttpPost]
        public async Task<IActionResult> BuyUpgrade(int gameId, string upgradeName)
        {
            var game = await _context.Games
                .Include(g => g.WareHouse)
                .ThenInclude(w => w.UpgradesPurchased)
                .FirstOrDefaultAsync(m => m.ID == gameId);

            if (game == null || game.WareHouse == null)
            {
                return Json(new { success = false, message = "A játék nem található." });
            }

            var jsonPath = Path.Combine(_env.WebRootPath, "data", "shop.json");
            var jsonString = await System.IO.File.ReadAllTextAsync(jsonPath);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);

            System.Text.Json.JsonElement? foundUpgrade = null;
            foreach (var item in doc.RootElement.GetProperty("Warehouse shop").EnumerateArray())
            {
                if (item.GetProperty("Name").GetString() == upgradeName)
                {
                    foundUpgrade = item;
                    break;
                }
            }

            if (foundUpgrade == null)
            {
                return Json(new { success = false, message = "A bővítés nem található." });
            }

            var price = foundUpgrade.Value.GetProperty("Price").GetInt32();
            var size = foundUpgrade.Value.GetProperty("Size").GetInt32();

            if (game.Balance < price)
            {
                return Json(new { success = false, message = "Nincs elegendő egyenleged" });
            }

            game.Balance -= price;
            game.WareHouse.StorgarSize += size;

            var existingPurchase = game.WareHouse.UpgradesPurchased.FirstOrDefault(u => u.UpgradeName == upgradeName);
            if (existingPurchase != null)
            {
                existingPurchase.Quantity += 1;
            }
            else
            {
                game.WareHouse.UpgradesPurchased.Add(new Robot_Simulation.Models.WarehouseUpgradePurchase
                {
                    UpgradeName = upgradeName,
                    Quantity = 1
                });
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, balance = game.Balance, newSize = game.WareHouse.StorgarSize });
        }
    }
}
