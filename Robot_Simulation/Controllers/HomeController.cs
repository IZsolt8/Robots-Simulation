using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Robot_Simulation.Data;
using Robot_Simulation.Models;
using System.Diagnostics;

namespace Robots_Simulation.Controllers
{
    public class HomeController : Controller
    {
        private readonly RobotSimulationContext _context;
        public HomeController(RobotSimulationContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewGame()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,GameName,Balance,WarehouseId")] Game game)
        {
            if (ModelState.IsValid)
            {
                var newWarehouse = new WareHouse();
                _context.Add(newWarehouse);
                await _context.SaveChangesAsync();

                game.WarehouseId = newWarehouse.ID;

                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WarehouseId"] = new SelectList(_context.WareHouses, "ID", "ID", game.WarehouseId);
            return View(game);
        }
        public async Task<IActionResult> LoadGame()
        {
            var savedGames = await _context.Games.Include(g => g.WareHouse).ToListAsync();
            return View(savedGames);
        }
        public IActionResult Exit()
        {
            return Redirect("https://google.com");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

