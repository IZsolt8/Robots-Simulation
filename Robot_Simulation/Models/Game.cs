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
    }
}
