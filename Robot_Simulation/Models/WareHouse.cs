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
        public int UsedSpace => Packages?.Count(p => !p.IsDelivered) ?? 0;

        [NotMapped]
        public int FreeSpace => Math.Max(0, StorgarSize - UsedSpace);

        [NotMapped]
        public int RobotMaintenanceFee => Robots?.Sum(r => r.MaintenanceFee) ?? 0;

        [NotMapped]
        public int WarehouseMaintenanceFee => (int)(StorgarSize * 0.3 * 200);

        [NotMapped]
        public int TotalMaintenanceFee => RobotMaintenanceFee + WarehouseMaintenanceFee;

        [NotMapped]
        public IEnumerable<Packages> PackagesWaitingForPacking => Packages?.Where(p => !p.Status && !p.IsDelivered) ?? Enumerable.Empty<Packages>();

        [NotMapped]
        public IEnumerable<Packages> PackedPackages => Packages?.Where(p => p.Status && !p.IsDelivered) ?? Enumerable.Empty<Packages>();

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
                    BatterySize = shopData.GetProperty("BatterySize").GetInt32(),
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
                    MaxChargingCapacity = shopData.TryGetProperty("MaxChargingCapacity", out var cap) ? cap.GetInt32() : 0,
                    Status = true,
                    WareHouseId = this.ID
                };
            }

            Robots.Add(newRobot);
            return newRobot;
        }

        public void ProcessDailyPacking(int currentDay, int hoursToProcess = 24)
        {
            for (int hour = 0; hour < hoursToProcess; hour++)
            {
                ProcessHourlyPacking(currentDay);
            }
        }

        public void ProcessHourlyPacking(int currentDay)
        {
            var packingRobots = Robots.OfType<PackingRobot>().ToList();
            var chargingRobots = Robots.OfType<ChargingRobot>().ToList();
            
            float totalChargingSpeed = chargingRobots.Sum(r => r.ChargingSpeed);

            var packagesToPack = GetPackagesToPack();

            ProcessCharging(chargingRobots, packingRobots);
            ProcessPacking(packingRobots, packagesToPack, currentDay, totalChargingSpeed);
        }

        private List<Packages> GetPackagesToPack()
        {
            return Packages
                .Where(p => p.Status == false)
                .OrderBy(p => p.Type == "romlandó" ? 0 : 1)
                .ThenBy(p => p.Type == "nem romlandó" ? p.CreatedOnDay : 0)
                .ToList();
        }

        private void ProcessCharging(List<ChargingRobot> chargingRobots, List<PackingRobot> packingRobots)
        {
            var robotsToCharge = packingRobots
                .Where(r => r.IsCharging)
                .OrderByDescending(r => r.BatteryLevel)
                .ToList();

            if (!robotsToCharge.Any()) return;

            var robotsAssignedThisHour = new HashSet<int>();

            foreach (var chargingRobot in chargingRobots)
            {
                var actualRobotsToCharge = GetRobotsForCharging(chargingRobot, robotsToCharge, robotsAssignedThisHour);

                foreach (var r in actualRobotsToCharge)
                {
                    robotsAssignedThisHour.Add(r.ID);
                }

                chargingRobot.ChargeRobots(actualRobotsToCharge);
            }
        }

        private List<PackingRobot> GetRobotsForCharging(ChargingRobot chargingRobot, List<PackingRobot> robotsToCharge, HashSet<int> robotsAssignedThisHour)
        {
            var unassignedRobots = robotsToCharge
                .Where(r => !robotsAssignedThisHour.Contains(r.ID))
                .ToList();

            if (!unassignedRobots.Any())
            {
                return new List<PackingRobot>();
            }

            int capacity = chargingRobot.MaxChargingCapacity > 0 ? chargingRobot.MaxChargingCapacity : 1;
            return unassignedRobots.Take(capacity).ToList();
        }

        private void ProcessPacking(List<PackingRobot> packingRobots, List<Packages> packagesToPack, int currentDay, float totalChargingSpeed)
        {
            foreach (var robot in packingRobots)
            {
                if (robot.IsCharging) continue;

                int maxPackagesThisHour = robot.PackingSpeed;
                int packedThisHour = 0;
                bool failedDueToBattery = false;

                for (int i = 0; i < packagesToPack.Count; i++)
                {
                    if (packedThisHour >= maxPackagesThisHour) break;

                    var pkg = packagesToPack[i];
                    if (TryPackPackage(robot, pkg, currentDay))
                    {
                        packagesToPack.RemoveAt(i);
                        i--;
                        packedThisHour++;
                    }
                    else
                    {
                        failedDueToBattery = true;
                        continue;
                    }
                }

                CheckAndSetChargingState(robot, packedThisHour, failedDueToBattery, totalChargingSpeed);
            }
        }

        private bool TryPackPackage(PackingRobot robot, Packages pkg, int currentDay)
        {
            if (robot.BatteryLevel >= pkg.BatteryCost)
            {
                robot.BatteryLevel -= pkg.BatteryCost;
                pkg.Status = true;
                pkg.IsUnderPacking = false;
                pkg.PackedOnDay = currentDay;
                return true;
            }
            return false;
        }

        private void CheckAndSetChargingState(PackingRobot robot, int packedThisHour, bool failedDueToBattery, float totalChargingSpeed)
        {
            if (!robot.IsCharging && totalChargingSpeed > 0)
            {
                if (robot.BatteryLevel <= 0 || (packedThisHour == 0 && failedDueToBattery))
                {
                    robot.IsCharging = true;
                }
            }
        }
    }
}
