using System.ComponentModel.DataAnnotations;

namespace Robot_Simulation.Models
{
    public class WareHouse
    {
        public int ID { get; set; }

        public int StorgarSize { get; set; } = 10; 

        public int MaitananceFee { get; set; } = 0; 
    }
}
