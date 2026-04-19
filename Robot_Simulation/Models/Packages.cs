using System.ComponentModel.DataAnnotations;

namespace Robot_Simulation.Models
{
    public class Packages
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public int StorageTime { get; set; }
        public bool Status { get; set; }
        public float BatteryCost { get; set; }



    }
}
