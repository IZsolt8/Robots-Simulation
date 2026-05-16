using System.ComponentModel.DataAnnotations.Schema;

namespace Robot_Simulation.Models
{
    public class PackingRobot : Robot
    {
        public float BatteryLevel { get; set; }
        public int PackingSpeed { get; set; }

        [NotMapped]
        public int BatterySize { get; set; }
    }
}
