namespace Robot_Simulation.Models
{
    public class ShopCardViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Img { get; set; } = string.Empty;
        public int OwnedCount { get; set; }
        public List<string> Stats { get; set; } = new();
        public string RobotType { get; set; } = string.Empty;
        public int GameId { get; set; }
    }
}
