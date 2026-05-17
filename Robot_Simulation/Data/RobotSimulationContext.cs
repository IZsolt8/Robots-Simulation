using Microsoft.EntityFrameworkCore;
using Robot_Simulation.Models;

namespace Robot_Simulation.Data
{
    public class RobotSimulationContext : DbContext
    {
        public RobotSimulationContext(DbContextOptions<RobotSimulationContext> options): base(options)
        {
        }

        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<WareHouse> WareHouses { get; set; } = null!;
        public DbSet<Robot> Robots { get; set; } = null!;
        public DbSet<WarehouseUpgrade> WarehouseUpgradePurchases { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Robot>()
                .HasDiscriminator<string>("robot_type")
                .HasValue<Robot>("robot")
                .HasValue<PackingRobot>("packing")
                .HasValue<ChargingRobot>("charging");

            modelBuilder.Entity<Robot>()
                .HasOne(r => r.WareHouse)
                .WithMany(w => w.Robots)
                .HasForeignKey(r => r.WareHouseId);
        }
    }
}