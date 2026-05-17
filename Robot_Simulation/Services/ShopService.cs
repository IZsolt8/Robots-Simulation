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
    }
}
