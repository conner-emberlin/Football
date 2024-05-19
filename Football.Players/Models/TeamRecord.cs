namespace Football.Players.Models
{
    public class TeamRecord
    {
        public TeamMap TeamMap { get; set; } = new();
        public int Season { get; set; }
        public int CurrentWeek { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
    }
}
