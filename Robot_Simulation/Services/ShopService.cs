using System.Text.Json;

namespace Robot_Simulation.Services
{
    public class ShopService
    {
        private readonly IWebHostEnvironment _env;

        public ShopService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<JsonElement?> GetItemDetailsAsync(string category, string itemName)
        {
            var jsonPath = Path.Combine(_env.WebRootPath, "data", "shop.json");
            var jsonString = await System.IO.File.ReadAllTextAsync(jsonPath);
            using var doc = JsonDocument.Parse(jsonString);

            if (!doc.RootElement.TryGetProperty(category, out var categoryElement))
            {
                return null;
            }

            foreach (var item in categoryElement.EnumerateArray())
            {
                if (item.TryGetProperty("Name", out var nameProp) && nameProp.GetString() == itemName)
                {
                    return item.Clone(); // Clone to preserve the element outside the Using block
                }
            }

            return null;
        }

        public async Task<int?> GetItemPriceAsync(string category, string itemName)
        {
            var item = await GetItemDetailsAsync(category, itemName);
            if (item.HasValue && item.Value.TryGetProperty("Price", out var priceProp))
            {
                return priceProp.GetInt32();
            }
            return null;
        }

        public async Task<List<Models.PackingRobot>> GetPackingRobotsAsync()
        {
            var jsonPath = Path.Combine(_env.WebRootPath, "data", "shop.json");
            var jsonString = await System.IO.File.ReadAllTextAsync(jsonPath);
            using var doc = JsonDocument.Parse(jsonString);
            
            if (doc.RootElement.TryGetProperty("PackingRobot", out var element))
            {
                return JsonSerializer.Deserialize<List<Models.PackingRobot>>(element.GetRawText()) ?? new List<Models.PackingRobot>();
            }
            return new List<Models.PackingRobot>();
        }

        public async Task<List<Models.ChargingRobot>> GetChargingRobotsAsync()
        {
            var jsonPath = Path.Combine(_env.WebRootPath, "data", "shop.json");
            var jsonString = await System.IO.File.ReadAllTextAsync(jsonPath);
            using var doc = JsonDocument.Parse(jsonString);

            if (doc.RootElement.TryGetProperty("ChargingRobot", out var element))
            {
                return JsonSerializer.Deserialize<List<Models.ChargingRobot>>(element.GetRawText()) ?? new List<Models.ChargingRobot>();
            }
            return new List<Models.ChargingRobot>();
        }

        public async Task<List<Models.ShopCardViewModel>> GetWarehouseUpgradesAsync(Models.WareHouse? wareHouse, int gameId)
        {
            var jsonPath = Path.Combine(_env.WebRootPath, "data", "shop.json");
            var jsonString = await System.IO.File.ReadAllTextAsync(jsonPath);
            using var doc = JsonDocument.Parse(jsonString);

            var warehouseCards = new List<Models.ShopCardViewModel>();
            if (doc.RootElement.TryGetProperty("Warehouse shop", out var element))
            {
                foreach (var item in element.EnumerateArray())
                {
                    var upgradeName = item.GetProperty("Name").GetString() ?? "";
                    var ownedCount = wareHouse?.GetUpgradeCount(upgradeName) ?? 0;

                    warehouseCards.Add(new Models.ShopCardViewModel
                    {
                        Name = upgradeName,
                        Price = item.GetProperty("Price").GetInt32(),
                        Img = item.GetProperty("Img").GetString() ?? "",
                        OwnedCount = ownedCount,
                        RobotType = "Upgrade",
                        GameId = gameId,
                        Stats = new List<string>
                        {
                            $"Raktárméret bővítés: +{item.GetProperty("Size").GetInt32()}"
                        }
                    });
                }
            }
            return warehouseCards;
        }
    }
}
