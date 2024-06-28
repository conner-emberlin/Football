namespace Football.Api.Models
{
    public class WeeklyProjectionErrorModel
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public string Position { get; set; } = "";
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public double FantasyPoints { get; set; }
        public double ProjectedPoints { get; set; }
        public double Error { get; set; }
    }
}
