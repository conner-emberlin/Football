using Football.Players.Models;
namespace Football.Fantasy.Analysis.Models
{
    public class MarketShare
    {
        public Player Player { get; set; } = new();
        public PlayerTeam PlayerTeam { get; set; } = new();
        public int Games { get; set; }
        public double TotalFantasy { get; set; }
        public double FantasyShare { get; set; }
        public double RushAttShare { get; set; }
        public double RushYdShare { get; set; }
        public double RushTDShare { get; set; }
        public double TargetShare { get; set; }
        public double ReceptionShare { get; set; }
        public double RecYdShare { get; set; }
        public double RecTDShare { get; set; }
    }

    public class TeamTotals
    {
        public TeamMap Team { get; set; } = new();
        public int Games { get; set; }
        public double TotalFantasy { get; set; }
        public double TotalFantasyQB { get; set; }
        public double TotalFantasyRB { get; set; }
        public double TotalFantasyWR { get; set; }
        public double TotalFantasyTE { get; set; }
        public double TotalRushAtt { get; set; }
        public double TotalRushYd { get; set; }
        public double TotalRushTd { get; set; }
        public double TotalTargets { get; set; }
        public double TotalReceptions { get; set; }
        public double TotalRecYds { get; set; }
        public double TotalRecTd { get; set; }
    }
    public class TargetShare
    {
        public TeamMap Team { get; set; } = new();
        public double RBTargetShare { get; set; }
        public double RBCompShare { get; set; }
        public double WRTargetShare { get; set; }
        public double WRCompShare { get; set; }
        public double TETargetShare { get; set; }
        public double TECompShare { get; set; }    
    }
}
