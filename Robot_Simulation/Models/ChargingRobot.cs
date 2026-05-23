namespace Robot_Simulation.Models
{
    public class ChargingRobot : Robot
    {
        public float ChargingSpeed { get; set; }
        public int MaxChargingCapacity { get; set; }

        public void ChargeRobots(List<PackingRobot> robotsToCharge)
        {
            if (robotsToCharge == null || robotsToCharge.Count == 0 || this.ChargingSpeed <= 0)
                return;

            int capacity = this.MaxChargingCapacity > 0 ? this.MaxChargingCapacity : 1;
            var actualRobotsToCharge = robotsToCharge.Take(capacity).ToList();

            if (actualRobotsToCharge.Count == 0) return;

            float chargePerRobot = this.ChargingSpeed / actualRobotsToCharge.Count;

            foreach (var robot in actualRobotsToCharge)
            {
                int maxBattery = robot.BatterySize > 0 ? robot.BatterySize : 10;
                robot.BatteryLevel += chargePerRobot;
                
                if (robot.BatteryLevel >= maxBattery)
                {
                    robot.BatteryLevel = maxBattery;
                    robot.IsCharging = false;
                }
            }
        }
    }
}
