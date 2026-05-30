using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Robot_Simulation.Data;

namespace Robot_Simulation.Controllers
{
    public class GamesController : Controller
    {
        private readonly RobotSimulationContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly Robot_Simulation.Services.ShopService _shopService;

        public GamesController(RobotSimulationContext context, IWebHostEnvironment env, Robot_Simulation.Services.ShopService shopService)
        {
            _context = context;
            _env = env;
            _shopService = shopService;
        }
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var game = await _context.Games
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Packages)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            var todaysPackages = game.WareHouse?.Packages
                .Where(p => p.CreatedOnDay == game.CurrentDay)
                .ToList() ?? new List<Models.Packages>();
            ViewBag.TodaysPackages = todaysPackages;
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
                    .ThenInclude(w => w.Packages)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            ViewBag.PackingRobots = await _shopService.GetPackingRobotsAsync();

            if (game.WareHouse != null)
            {
                ViewBag.OwnedCounts = game.WareHouse.GetOwnedRobotCounts();
            }
            else
            {
                ViewBag.OwnedCounts = new Dictionary<string, int>();
            }

            return View(game);
        }

        public async Task<IActionResult> WaitingPackages(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var game = await _context.Games
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Packages)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        public async Task<IActionResult> PackedPackages(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var game = await _context.Games
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Packages)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
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
                    .ThenInclude(w => w.Packages)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.UpgradesPurchased)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            ViewBag.WarehouseUpgrades = await _shopService.GetWarehouseUpgradesAsync(game.WareHouse, game.ID);

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
                    .ThenInclude(w => w.Packages)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            ViewBag.ChargingRobots = await _shopService.GetChargingRobotsAsync();

            if (game.WareHouse != null)
            {
                ViewBag.OwnedCounts = game.WareHouse.GetOwnedRobotCounts();
            }
            else
            {
                ViewBag.OwnedCounts = new Dictionary<string, int>();
            }

            return View(game);
        }

        [HttpPost]
        public async Task<IActionResult> BuyRobot(int gameId, string robotType, string robotName)
        {
            var game = await _context.Games
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Packages)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == gameId);

            if (game == null || game.WareHouse == null)
            {
                return Json(new { success = false, message = "A játék nem található." });
            }

            var foundRobot = await _shopService.GetItemDetailsAsync(robotType, robotName);

            if (foundRobot == null)
            {
                return Json(new { success = false, message = "A robot nem található." });
            }

            var price = foundRobot.Value.GetProperty("Price").GetInt32();

            if (!game.CanAfford(price))
            {
                return Json(new { success = false, message = "Nincs elegendő egyenlege" });
            }

            game.DeductBalance(price);
            game.WareHouse.AddRobotFromShop(robotType, robotName, foundRobot.Value);

            await _context.SaveChangesAsync();

            return Json(new { success = true, balance = game.Balance, maintenanceFee = game.WareHouse.TotalMaintenanceFee, storageSize = game.WareHouse.StorgarSize, usedSpace = game.WareHouse.UsedSpace, freeSpace = game.WareHouse.FreeSpace });
        }

        [HttpPost]
        public async Task<IActionResult> SellRobot(int gameId, string robotType, string robotName)
        {
            var game = await _context.Games
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == gameId);

            if (game == null || game.WareHouse == null)
            {
                return Json(new { success = false, message = "A játék nem található." });
            }

            var foundRobot = await _shopService.GetItemDetailsAsync(robotType, robotName);

            if (foundRobot == null)
            {
                return Json(new { success = false, message = "A robot nem található a boltban." });
            }

            var price = foundRobot.Value.GetProperty("Price").GetInt32();
            var sellCost = (int)(price * 0.4);

            if (!game.CanAfford(sellCost))
            {
                return Json(new { success = false, message = "Nincs elegendő egyenlege az eladáshoz." });
            }

            var robotToRemove = game.WareHouse.Robots.FirstOrDefault(r => r.Name == robotName);
            if (robotToRemove == null)
            {
                return Json(new { success = false, message = "Nincs ilyen robot a raktárban." });
            }

            game.DeductBalance(sellCost);
            game.WareHouse.Robots.Remove(robotToRemove);
            _context.Robots.Remove(robotToRemove);

            await _context.SaveChangesAsync();

            return Json(new { success = true, balance = game.Balance, maintenanceFee = game.WareHouse.TotalMaintenanceFee, storageSize = game.WareHouse.StorgarSize, usedSpace = game.WareHouse.UsedSpace, freeSpace = game.WareHouse.FreeSpace });
        }

        [HttpPost]
        public async Task<IActionResult> BuyUpgrade(int gameId, string upgradeName)
        {
            var game = await _context.Games
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Packages)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.UpgradesPurchased)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == gameId);

            if (game == null || game.WareHouse == null)
            {
                return Json(new { success = false, message = "A játék nem található." });
            }

            var foundUpgrade = await _shopService.GetItemDetailsAsync("Warehouse shop", upgradeName);

            if (foundUpgrade == null)
            {
                return Json(new { success = false, message = "A bővítés nem található." });
            }

            var price = foundUpgrade.Value.GetProperty("Price").GetInt32();
            var size = foundUpgrade.Value.GetProperty("Size").GetInt32();

            if (!game.CanAfford(price))
            {
                return Json(new { success = false, message = "Nincs elegendő egyenleged" });
            }

            game.DeductBalance(price);
            game.WareHouse.AddUpgrade(upgradeName, size);

            await _context.SaveChangesAsync();

            return Json(new { success = true, balance = game.Balance, maintenanceFee = game.WareHouse.TotalMaintenanceFee, storageSize = game.WareHouse.StorgarSize, usedSpace = game.WareHouse.UsedSpace, freeSpace = game.WareHouse.FreeSpace });
        }

        [HttpPost]
        public async Task<IActionResult> NextDay(int gameId, int remainingHours = 24)
        {
            var game = await _context.Games
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Packages)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == gameId);

            if (game == null || game.WareHouse == null)
            {
                return NotFound();
            }

            game.WareHouse.ProcessDailyPacking(game.CurrentDay, remainingHours);

            game.ProcessDeliveries();

            game.NextDay();

            var packageJsonPath = Path.Combine(_env.WebRootPath, "data", "Package.json");
            var newPackages = Models.Packages.GenerateForWarehouse(game.WareHouse, packageJsonPath, game.CurrentDay);
            _context.Packages.AddRange(newPackages);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id = gameId });
        }

        [HttpPost]
        public async Task<IActionResult> NextHour(int gameId)
        {
            var game = await _context.Games
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Packages)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .FirstOrDefaultAsync(m => m.ID == gameId);

            if (game == null || game.WareHouse == null)
            {
                return NotFound();
            }

            game.WareHouse.ProcessHourlyPacking(game.CurrentDay);
            game.CurrentHour++;
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id = gameId });
        }

        [HttpPost]
        public async Task<IActionResult> ExitGame(int gameId)
        {
            var game = await _context.Games.FirstOrDefaultAsync(m => m.ID == gameId);
            if (game != null)
            {
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction("Index", "Home");
        }
    }
}
