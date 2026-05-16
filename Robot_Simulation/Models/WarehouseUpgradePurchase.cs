using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Robot_Simulation.Models
{
    public class WarehouseUpgradePurchase
    {
        [Key]
        public int ID { get; set; }

        public int WareHouseId { get; set; }
        
        [ForeignKey("WareHouseId")]
        public virtual WareHouse WareHouse { get; set; } = null!;

        public string UpgradeName { get; set; } = string.Empty;

        public int Quantity { get; set; } = 0;
    }
}
