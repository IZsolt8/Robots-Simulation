using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Robot_Simulation.Models
{
    public class Packages
    {
        public int ID { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Price { get; set; }

        public int StorageTime { get; set; }

        public bool Status { get; set; } = false;

        public bool IsUnderPacking { get; set; } = false;

        public bool IsDelivered { get; set; } = false;

        public float BatteryCost { get; set; }

        public int WareHouseId { get; set; }

        public int CreatedOnDay { get; set; } = 0;

        public int? PackedOnDay { get; set; }

        [ForeignKey("WareHouseId")]
        public virtual WareHouse? WareHouse { get; set; }

        public bool IsReadyForDelivery(int currentDay)
        {
            if (!Status) return false;
            if (IsDelivered) return false;
            
            int daysSincePacked = currentDay - (PackedOnDay ?? CreatedOnDay);
            return daysSincePacked >= StorageTime;
        }

        public static List<Packages> GenerateForWarehouse(WareHouse wareHouse, string packageJsonPath, int currentDay)
        {
            var templates = LoadTemplatesFromJson(packageJsonPath);
            if (!templates.Any() || wareHouse.FreeSpace <= 0)
                return new List<Packages>();

            int count = CalculateGenerationCount(wareHouse.FreeSpace);
            var rng = new Random();
            var result = new List<Packages>();

            for (int i = 0; i < count; i++)
            {
                var template = templates[rng.Next(templates.Count)];
                var package = CreatePackageFromTemplate(template, wareHouse.ID, currentDay, rng);
                result.Add(package);
            }

            return result;
        }

        private static List<PackageTemplate> LoadTemplatesFromJson(string packageJsonPath)
        {
            if (!File.Exists(packageJsonPath))
                return new List<PackageTemplate>();

            var jsonString = File.ReadAllText(packageJsonPath);

            if (string.IsNullOrWhiteSpace(jsonString))
                return new List<PackageTemplate>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<PackageTemplate>>(jsonString, options) 
                   ?? new List<PackageTemplate>();
        }

        private static int CalculateGenerationCount(int freeSpace)
        {
            return Math.Max(1, (int)Math.Floor(freeSpace * 0.6));
        }

        private static Packages CreatePackageFromTemplate(PackageTemplate template, int warehouseId, int currentDay, Random rng)
        {
            int storageTime = rng.Next(1, 9);
            int basePrice = rng.Next(1700, 3901);
            
            float multiplier = CalculatePriceMultiplier(storageTime);
            int finalPrice = (int)(basePrice * multiplier);
            float batteryCost = rng.Next(1, 5);

            return new Packages
            {
                Type = template.Type,
                Name = template.Name,
                StorageTime = storageTime,
                Price = finalPrice,
                BatteryCost = batteryCost,
                Status = false,
                WareHouseId = warehouseId,
                CreatedOnDay = currentDay
            };
        }

        private static float CalculatePriceMultiplier(int storageTime)
        {
            if (storageTime == 8)
                return 2f;
            if (storageTime >= 4)
                return 1.5f;
            
            return 1f;
        }

        private class PackageTemplate
        {
            public string Type { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }
    }
}
