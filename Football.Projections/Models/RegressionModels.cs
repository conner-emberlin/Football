namespace Football.Projections.Models
{
    public class QBModelSeason
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double PassingAttemptsPerGame { get; set; }
        public double PassingYardsPerGame { get; set; }
        public double PassingTouchdownsPerGame { get; set; }
        public double RushingAttemptsPerGame { get; set; }
        public double RushingYardsPerGame { get; set; }
        public double RushingTouchdownsPerGame { get; set; }
        public double SacksPerGame { get; set; }
    }
    public class RBModelSeason
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double RushingAttemptsPerGame { get; set; }
        public double RushingYardsPerGame { get; set; }
        public double RushingYardsPerAttempt { get; set; }
        public double RushingTouchdownsPerGame { get; set; }
        public double ReceptionsPerGame { get; set; }
        public double ReceivingYardsPerGame { get; set; }
        public double ReceivingTouchdownsPerGame { get; set; }
    }
    public class WRModelSeason
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double TargetsPerGame { get; set; }
        public double ReceptionsPerGame { get; set; }
        public double YardsPerGame { get; set; }
        public double YardsPerReception { get; set; }
        public double TouchdownsPerGame { get; set; }
    }
    public class TEModelSeason
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double TargetsPerGame { get; set; }
        public double ReceptionsPerGame { get; set; }
        public double YardsPerGame { get; set; }
        public double YardsPerReception { get; set; }
        public double TouchdownsPerGame { get; set; }
    }

    public class QBModelWeek : QBModelSeason
    {
        public int Week { get; set; }
        public double ProjectedPoints { get; set; }
        public double FiveThirtyEightValue { get; set; }
        public double PasserRating { get; set; }
    }

    public class RBModelWeek : RBModelSeason
    {
        public int Week { get; set; }
        public double ProjectedPoints { get; set; }
        public double RBTargetShare { get; set; }
    }

    public class WRModelWeek : WRModelSeason
    {
        public int Week { get; set; }
        public double ProjectedPoints { get; set; }
    }
    public class TEModelWeek : TEModelSeason
    {
        public int Week { get; set; }
        public double ProjectedPoints { get; set; }
    }

    public class DSTModelWeek
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public double SacksPerGame { get; set; }
        public double IntsPerGame { get; set; }
        public double FumRecPerGame { get; set; }
        public double TotalTDPerGame { get; set; }
        public double SaftiesPerGame { get; set; }
        public double YardsAllowedPerGame { get; set; }
        public double StrengthOfSchedule { get; set; }
    }
}
