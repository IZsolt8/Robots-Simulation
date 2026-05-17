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
        public string Name { get; set; } = string.Empty;

        [NotMapped]
        public int Size { get; set; }

        [NotMapped]
        public int Price { get; set; }

        [NotMapped]
        public string Img { get; set; } = string.Empty;

        public virtual ICollection<Robot> Robots { get; set; } = new List<Robot>();
        public virtual ICollection<WarehouseUpgrade> UpgradesPurchased { get; set; } = new List<WarehouseUpgrade>();
    }
}
