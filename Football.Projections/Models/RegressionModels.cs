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
}
