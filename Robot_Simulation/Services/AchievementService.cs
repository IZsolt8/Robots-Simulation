using Microsoft.EntityFrameworkCore;
using Robot_Simulation.Data;
using Robot_Simulation.Models;
using System.Text.Json;

namespace Robot_Simulation.Services
{
    public class AchievementService
    {
        private readonly RobotSimulationContext _context;
        private readonly IWebHostEnvironment _env;

        public AchievementService(RobotSimulationContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<string>> CheckAchievementsAsync(int gameId)
        {
            var game = await _context.Games
                .Include(g => g.UnlockedAchievements)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Robots)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.Packages)
                .Include(g => g.WareHouse)
                    .ThenInclude(w => w.UpgradesPurchased)
                .FirstOrDefaultAsync(g => g.ID == gameId);

            if (game == null) return new List<string>();

            var jsonPath = Path.Combine(_env.WebRootPath, "data", "achievements.json");
            if (!File.Exists(jsonPath)) return new List<string>();

            var json = await File.ReadAllTextAsync(jsonPath);
            var doc = JsonDocument.Parse(json);
            var achievementsArray = doc.RootElement.GetProperty("achievements").EnumerateArray();

            var newlyUnlocked = new List<string>();
            var unlockedIds = game.UnlockedAchievements.Select(a => a.AchievementId).ToHashSet();

            int storageSize = game.WareHouse?.StorgarSize ?? 10;
            int upgradeCount = game.WareHouse?.UpgradesPurchased?.Sum(u => u.Quantity) ?? 0;
            int packerCount = (game.WareHouse?.Robots?.OfType<PackingRobot>().Count() ?? 1) - 1;
            int chargerCount = (game.WareHouse?.Robots?.OfType<ChargingRobot>().Count() ?? 1) - 1;
            int packagesPacked = game.WareHouse?.Packages?.Count(p => p.Status) ?? 0;
            float energyCharged = game.WareHouse?.Packages?.Where(p => p.Status).Sum(p => p.BatteryCost) ?? 0f;

            foreach (var ach in achievementsArray)
            {
                var id = ach.GetProperty("id").GetString();
                if (id == null || unlockedIds.Contains(id)) continue;

                var parts = id.Split('_');
                if (parts.Length < 2) continue;

                var type = parts[0];
                var valueStr = parts[1];

                long targetValue = 0;
                if (valueStr.EndsWith("k")) targetValue = long.Parse(valueStr.TrimEnd('k')) * 1000;
                else if (valueStr.EndsWith("m")) targetValue = long.Parse(valueStr.TrimEnd('m')) * 1000000;
                else targetValue = long.Parse(valueStr);

                bool isUnlocked = false;
                switch (type)
                {
                    case "expansion":
                        isUnlocked = targetValue == 1
                            ? upgradeCount >= 1
                            : storageSize >= targetValue;
                        break;
                    case "packer": isUnlocked = packerCount >= targetValue; break;
                    case "charger": isUnlocked = chargerCount >= targetValue; break;
                    case "money": isUnlocked = game.Balance >= targetValue; break;
                    case "packages": isUnlocked = packagesPacked >= targetValue; break;
                    case "energy": isUnlocked = energyCharged >= targetValue; break;
                    case "days": isUnlocked = game.CurrentDay >= targetValue; break;
                }

                if (isUnlocked)
                {
                    newlyUnlocked.Add(id);
                    game.UnlockedAchievements.Add(new UnlockedAchievement { AchievementId = id });
                }
            }

            if (newlyUnlocked.Any())
            {
                await _context.SaveChangesAsync();
            }

            return newlyUnlocked;
        }
    }
}
