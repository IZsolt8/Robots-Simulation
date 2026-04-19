using System.ComponentModel.DataAnnotations;

namespace Robot_Simulation.Models
{
    public class NewGame
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Game name")]
        public string GameName { get; set; } = string.Empty;
    }
}
