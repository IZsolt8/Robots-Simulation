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
    }
}