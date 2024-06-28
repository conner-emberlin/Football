using Football.Players.Models;

namespace Football.Api.Models
{
    public class MarketShareModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public string Team { get; set; } = "";
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
}
