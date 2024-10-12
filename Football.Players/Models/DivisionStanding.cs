namespace Football.Players.Models
{
    public class DivisionStanding
    {
        public string Division { get; set; } = "";
        public int TeamId { get; set; }
        public string TeamDescription { get; set; } = "";
        public int Standing { get; set; }
    }
}
