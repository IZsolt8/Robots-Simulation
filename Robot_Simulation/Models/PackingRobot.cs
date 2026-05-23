using System.ComponentModel.DataAnnotations.Schema;

namespace Robot_Simulation.Models
{
    public class PackingRobot : Robot
    {
        public float BatteryLevel { get; set; }
        public int PackingSpeed { get; set; }

        public int BatterySize { get; set; }
        public bool IsCharging { get; set; } = false;

        public void PackPackages(List<Packages> packagesToPack, int currentDay)
        {
            int maxPackagesPerDay = this.PackingSpeed * 24;
            int packedToday = 0;

            foreach (var pkg in packagesToPack)
            {
                if (pkg.Status) continue;
                if (packedToday >= maxPackagesPerDay) break;

                if (this.BatteryLevel >= pkg.BatteryCost)
                {
                    this.BatteryLevel -= pkg.BatteryCost;
                    pkg.Status = true;
                    pkg.IsUnderPacking = false;
                    pkg.PackedOnDay = currentDay;
                    packedToday++;
                }
            }
        }
    }
}
