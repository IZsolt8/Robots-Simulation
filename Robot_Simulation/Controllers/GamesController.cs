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

            return Json(new { success = true, balance = game.Balance, newSize = game.WareHouse.StorgarSize });
        }
    }
}
