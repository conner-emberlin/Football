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
        public double YearsExperience { get; set; }
        public double InterceptionsPerGame { get; set; }
        public double FumblesPerGame { get; set; }
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
        public double YearsExperience { get; set; }
        public double FumblesPerGame { get; set; }
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
        public double YearsExperience { get; set; }
        public double FumblesPerGame { get; set; }
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
        public double YearsExperience { get; set; }
        public double FumblesPerGame { get; set; }
    }

    public class QBModelWeek 
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public double PassingAttemptsPerGame { get; set; }
        public double PassingCompletionsPerGame { get; set; }
        public double PassingYardsPerAttempt { get; set; }
        public double RushingAttemptsPerGame { get; set; }
        public double RushingYardsPerAttempt { get; set; }
        public double SacksPerGame { get; set; }
        public double SnapsPerGame { get; set; }
    }

    public class RBModelWeek 
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public double RushingAttemptsPerGame { get; set; }
        public double RushingYardsPerAttempt { get; set; }
        public double TargetsPerGame { get; set; }
        public double ReceivingYardsPerReception { get; set; }
        public double SnapsPerGame { get; set; }
    }

    public class WRModelWeek
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public double RushingAttemptsPerGame { get; set; }
        public double RushingYardsPerAttempt { get; set; }
        public double TargetsPerGame { get; set; }
        public double ReceivingYardsPerReception { get; set; }
        public double SnapsPerGame { get; set; }
        public double ReceptionRate { get; set; }
    }
    public class TEModelWeek 
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public double RushingAttemptsPerGame { get; set; }
        public double RushingYardsPerAttempt { get; set; }
        public double TargetsPerGame { get; set; }
        public double ReceivingYardsPerReception { get; set; }
        public double SnapsPerGame { get; set; }
        public double ReceptionRate { get; set;}
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
    }

    public class KModelWeek
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public double ExtraPointsPerGame { get; set; }
        public double ExtraPointAttsPerGame { get; set; }
        public double FieldGoalsPerGame { get; set; }
        public double FieldGoalAttemptsPerGame { get; set; }
        public double FiftyPlusFieldGoalsPerGame { get; set; }
    }
}
