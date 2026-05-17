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

        public float BatteryCost { get; set; }

        public int WareHouseId { get; set; }

        public int CreatedOnDay { get; set; } = 0;

        [ForeignKey("WareHouseId")]
        public virtual WareHouse? WareHouse { get; set; }

        public static List<Packages> GenerateForWarehouse(WareHouse wareHouse, string packageJsonPath, int currentDay)
        {
            if (!File.Exists(packageJsonPath))
                return new List<Packages>();

            var jsonString = File.ReadAllText(packageJsonPath);

            if (string.IsNullOrWhiteSpace(jsonString))
                return new List<Packages>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var templates = JsonSerializer.Deserialize<List<PackageTemplate>>(jsonString, options)
                ?? new List<PackageTemplate>();

            if (templates.Count == 0)
                return new List<Packages>();

            var rng = new Random();

            if (wareHouse.FreeSpace <= 0) return new List<Packages>();

            int count = Math.Max(1, (int)Math.Floor(wareHouse.FreeSpace * 0.3));

            var result = new List<Packages>();
            for (int i = 0; i < count; i++)
            {
                var template = templates[rng.Next(templates.Count)];
                int storageTime = rng.Next(1, 9);
                int basePrice = rng.Next(30, 3901);

                float multiplier = 1f;
                if (storageTime == 8)
                    multiplier = 2f;
                else if (storageTime >= 4)
                    multiplier = 1.5f;

                int finalPrice = (int)(basePrice * multiplier);
                float batteryCost = rng.Next(1, 5);

                result.Add(new Packages
                {
                    Type = template.Type,
                    Name = template.Name,
                    StorageTime = storageTime,
                    Price = finalPrice,
                    BatteryCost = batteryCost,
                    Status = false,
                    WareHouseId = wareHouse.ID,
                    CreatedOnDay = currentDay
                });
            }

            return result;
        }

        private class PackageTemplate
        {
            public string Type { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }
    }
}
