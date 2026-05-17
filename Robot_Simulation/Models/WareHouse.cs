using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Robot_Simulation.Models
{
    public class WareHouse
    {
        public int ID { get; set; }

        public int StorgarSize { get; set; } = 10;

        public int MaitananceFee { get; set; } = 0;

        [NotMapped]
        public int UsedSpace => Packages?.Count(p => !p.Status) ?? 0;

        [NotMapped]
        public int FreeSpace => Math.Max(0, StorgarSize - UsedSpace);

        [NotMapped]
        public IEnumerable<Packages> PackagesWaitingForPacking => Packages?.Where(p => !p.Status) ?? Enumerable.Empty<Packages>();

        [NotMapped]
        public IEnumerable<Packages> PackedPackages => Packages?.Where(p => p.Status) ?? Enumerable.Empty<Packages>();

        [NotMapped]
        public string Name { get; set; } = string.Empty;

        [NotMapped]
        public int Size { get; set; }

        [NotMapped]
        public int Price { get; set; }

        [NotMapped]
        public string Img { get; set; } = string.Empty;

        public virtual ICollection<Robot> Robots { get; set; } = new List<Robot>();
        public virtual ICollection<WarehouseUpgrade> UpgradesPurchased { get; set; } = new List<WarehouseUpgrade>();
        public virtual ICollection<Packages> Packages { get; set; } = new List<Packages>();

        public Dictionary<string, int> GetOwnedRobotCounts()
        {
            var counts = new Dictionary<string, int>();
            foreach (var robot in Robots)
            {
                if (counts.ContainsKey(robot.Name))
                    counts[robot.Name]++;
                else
                    counts[robot.Name] = 1;
            }
            return counts;
        }

        public int GetUpgradeCount(string upgradeName)
        {
            return UpgradesPurchased.FirstOrDefault(u => u.UpgradeName == upgradeName)?.Quantity ?? 0;
        }

        public void AddUpgrade(string upgradeName, int sizeIncrease)
        {
            StorgarSize += sizeIncrease;

            var existingPurchase = UpgradesPurchased.FirstOrDefault(u => u.UpgradeName == upgradeName);
            if (existingPurchase != null)
            {
                existingPurchase.Quantity += 1;
            }
            else
            {
                UpgradesPurchased.Add(new WarehouseUpgrade
                {
                    UpgradeName = upgradeName,
                    Quantity = 1
                });
            }
        }

        public Robot AddRobotFromShop(string robotType, string robotName, System.Text.Json.JsonElement shopData)
        {
            Robot newRobot;
            if (robotType == "PackingRobot")
            {
                newRobot = new PackingRobot
                {
                    Name = robotName,
                    MaintenanceFee = shopData.GetProperty("MaintenanceFee").GetInt32(),
                    PackingSpeed = shopData.GetProperty("PackingSpeed").GetInt32(),
                    BatteryLevel = shopData.GetProperty("BatterySize").GetInt32(),
                    Status = true,
                    WareHouseId = this.ID
                };
            }
            else
            {
                newRobot = new ChargingRobot
                {
                    Name = robotName,
                    MaintenanceFee = shopData.GetProperty("MaintenanceFee").GetInt32(),
                    ChargingSpeed = (float)shopData.GetProperty("ChargingSpeed").GetDouble(),
                    Status = true,
                    WareHouseId = this.ID
                };
            }

            Robots.Add(newRobot);
            return newRobot;
        }
    }
}
