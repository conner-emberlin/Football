namespace Football.Models
{
    public class Season
    {
        public int CurrentSeason { get; set; }
        public int Games { get; set; }
    }

    public class ReplacementLevels
    {
        public int ReplacementLevelQB { get; set; }
        public int ReplacementLevelRB { get; set; }
        public int ReplacementLevelWR { get; set; }
        public int ReplacementLevelTE { get; set; }
    }

    public class FantasyScoring
    {
        public int PointsPerReception { get; set; }
        public int PointsPerPassingTouchdown { get; set; }
        public int PointsPerInterception { get; set; }
        public int PointsPerFumble { get; set; }
        public int PointsPerTouchdown { get; set; }
        public double PointsPerPassingYard { get; set; }
        public double PointsPerYard { get; set; }
        public double PointsPerSack { get; set; }
        public double PointsPerSafety { get; set; }
        public double ZeroPointsAllowed { get; set; }
        public double OneToSixPointsAllowed { get; set; }
        public double SevenToThirteenPointsAllowed { get; set; }
        public double FourteenToSeventeenPointsAllowed { get; set; }
        public double EighteenToTwentySevenPointsAllowed { get; set; }
        public double TwentyEightToThirtyFourPointsAllowed { get; set; }
        public double ThirtyFiveToFourtyFivePointsAllowed { get; set; }
        public double FourtySixOrMorePointsAllowed { get; set; }
    }

    public class ProjectionLimits
    {
        public int QBProjections { get; set; }
        public int RBProjections { get; set; }
        public int WRProjections { get; set; }
        public int TEProjections { get; set; }
    }

    public class Starters
    {
        public int QBStarters { get; set; }
        public int RBStarters { get; set; }
        public int WRStarters { get; set; }
        public int TEStarters { get; set; }
    }
    public class Tunings
    {
        public int RBFloor { get; set; }
        public double LeadRBFactor { get; set; }
        public double Weight { get; set; }
        public double SecondYearWRLeap { get; set; }
        public double SecondYearRBLeap { get; set; }
        public double SecondYearQBLeap { get; set; }
        public double NewQBFloor { get; set; }
        public double NewQBCeiling { get; set; }
    }

    public class WeeklyTunings
    {
        public double RecentWeekWeight { get; set; }
        public double ProjectionWeight { get; set; }
        public double TamperedMin { get; set; }
        public double TamperedMax { get; set; } 
    }

    public class MockDraftSettings
    {
        public int QBStarters { get; set; }
        public int RBStarters { get; set; }
        public int WRStarters { get; set; }
        public int TEStarters { get; set; }
        public int QBLimit { get; set; }
        public int RBLimit { get; set; }
        public int WRLimit { get; set; }
        public int TELimit { get; set; }
        public int BenchSpots { get; set; }
    }

    public class WaiverWireSettings
    {
        public int RostershipMax { get; set; }
        public int GoodWeekFantasyPointsQB { get; set; }
        public int GoodWeekFantasyPointsRB { get; set; }
        public int GoodWeekFantasyPointsWR { get; set; }
        public int GoodWeekFantasyPointsTE { get; set; }
        public int GoodWeekMinimum { get; set; }
    }

    public class BoomBustSettings
    {
        public double QBBoom { get; set; }
        public double QBBust { get; set; }
        public double RBBoom { get; set; }
        public double RBBust { get; set; }
        public double WRBoom { get; set; }
        public double WRBust { get; set;}
        public double TEBoom { get; set; }
        public double TEBust { get; set;}
    }

    public class GeoDistance
    {
        public int RadiusMiles { get; set; }
    }
}
