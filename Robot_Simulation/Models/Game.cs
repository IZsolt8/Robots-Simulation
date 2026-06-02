using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Robot_Simulation.Models
{
    public class Game
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Game name")]
        public string GameName { get; set; } = string.Empty;

        public int Balance { get; set; } = 20000;

        public int WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual WareHouse? WareHouse { get; set; }

        public virtual ICollection<UnlockedAchievement> UnlockedAchievements { get; set; } = new List<UnlockedAchievement>();

        public int CurrentDay { get; set; } = 0;
        public int CurrentHour { get; set; } = 0;

        public void ProcessDeliveries()
        {
            if (WareHouse == null || WareHouse.Packages == null) return;

            var readyPackages = WareHouse.Packages
                .Where(p => p.IsReadyForDelivery(CurrentDay))
                .ToList();

            foreach (var pkg in readyPackages)
            {
                Balance += pkg.Price;
                pkg.IsDelivered = true;
            }
        }

        public void NextDay()
        {
            CurrentDay++;
            CurrentHour = 0;
            if (WareHouse != null)
            {
                int warehouseFee = (int)(WareHouse.StorgarSize * 0.3 * 200);
                int robotFee = WareHouse.Robots?.Sum(r => r.MaintenanceFee) ?? 0;
                Balance -= (warehouseFee + robotFee);
            }
        }

        public bool CanAfford(int price)
        {
            return Balance >= price;
        }

        public void DeductBalance(int price)
        {
            if (!CanAfford(price))
            {
                throw new InvalidOperationException("Nincs elegendő egyenleg!");
            }
            Balance -= price;
        }
    }
}
