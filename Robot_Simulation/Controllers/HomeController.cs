using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Robot_Simulation.Data;
using Robot_Simulation.Models;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        // POST: Naplo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Naplo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(n => n.WareHouse)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View("LoadGame");
        }

        // POST: Naplo/Delete/5
        [HttpPost, ActionName("DeleteGame")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game != null)
            {
                var warehouse = await _context.WareHouses.FindAsync(game.WarehouseId);
                if (warehouse != null)
                {
                    _context.WareHouses.Remove(warehouse);
                }

                _context.Games.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(LoadGame));
        }

        public async Task<IActionResult> LoadGame()
        {
            var savedGames = await _context.Games
                .Include(g => g.WareHouse)
                .OrderByDescending(g => g.ID)
                .ToListAsync();

            return View(savedGames);
        }

        public async Task<IActionResult> Search(string? name)
        {
            var game = _context.Games.Include(g => g.WareHouse).AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                game = game
                .Where(p => p.GameName!.ToLower().Contains(name.ToLower()));
                ViewData["GameNameFilter"] = name;
            }

            var search = await game
            .OrderByDescending(g => g.ID)
            .ToListAsync();
            return View("LoadGame", search);
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

