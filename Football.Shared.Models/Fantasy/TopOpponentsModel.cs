namespace Football.Shared.Models.Fantasy
{
    public class TopOpponentsModel
    {
        public string TopOpponentTeamDescription { get; set; } = "";
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public int Season { get; set; }
        public int Week { get; set; }
        public double FantasyPoints { get; set; }
        public string Team { get; set; } = "";
        public double AverageFantasy { get; set; }
    }
}
