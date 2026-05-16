using System.ComponentModel.DataAnnotations.Schema;

namespace Robot_Simulation.Models
{
    public class Robot
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public int MaintenanceFee { get; set; }

        [NotMapped]
        public int Price { get; set; }

        [NotMapped]
        public string Img { get; set; } = string.Empty;

        public int? WareHouseId { get; set; }
        public virtual WareHouse? WareHouse { get; set; }
    }
}
