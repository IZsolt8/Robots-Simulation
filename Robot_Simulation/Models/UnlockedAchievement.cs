using System.ComponentModel.DataAnnotations;

namespace Robot_Simulation.Models
{
    public class UnlockedAchievement
    {
        public int ID { get; set; }
        
        public int GameId { get; set; }
        public virtual Game Game { get; set; } = null!;
        
        [Required]
        public string AchievementId { get; set; } = string.Empty;
    }
}
