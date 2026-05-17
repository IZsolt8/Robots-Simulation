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
