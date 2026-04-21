using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Robot_Simulation.Data;

namespace Robot_Simulation.Controllers
{
    public class GamesController : Controller
    {
        private readonly RobotSimulationContext _context;
        public GamesController(RobotSimulationContext context)
        {
            _context = context;
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
    }
}
