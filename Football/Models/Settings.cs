using System.Text.Json;

namespace Football.Models
{
    public class Season
    {
        public int CurrentSeason { get; set; }
        public int Games { get; set; }
        public int Weeks { get; set; }
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
        public double ExtraPoint { get; set; }
        public double ExtraPointMissed { get; set; }
        public double FGMissed { get; set; }
        public double FGLessThanFourty { get; set; }
        public double FGFourtyFifty { get; set; }
        public double FGGreaterThanFifty { get; set; }
    }

    public class ProjectionLimits
    {
        public int QBProjections { get; set; }
        public int RBProjections { get; set; }
        public int WRProjections { get; set; }
        public int TEProjections { get; set; }
        public int DSTProjections { get; set; }
        public int KProjections { get; set; }
        public int ErrorAdjustmentWeek { get; set; }
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
        public int SeasonDataTrimmingGames { get; set; }
        public double AverageQBProjection { get; set; }
        public double NewWRMinPoints { get; set; }
        public double NewWRAdjustmentFactor { get; set; }
        public double ExistingWRAdjustmentFactor { get; set; }
        public double NewRBMinPoints { get; set; }
        public double NewRBAdjustmentFactor { get; set; }
        public double ExistingRBAdjustmentFactor { get; set; }
        public double RushingQBThreshold { get; set; }
        public double ReceivingRBThreshold { get; set; }
        public double BackupQBAdjustmentMax { get; set; }
        public double VetQBNewTeamYears { get; set; }
        public double VetQBNewTeamFactor { get; set; }
    }

    public class WeeklyTunings
    {
        public double RecentWeekWeight { get; set; }
        public double ProjectionWeight { get; set; }
        public double TamperedMin { get; set; }
        public double TamperedMax { get; set; }
        public double MinWeekWeighted { get; set; }
        public int RecentWeeks { get; set; }
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
        public double WRBust { get; set; }
        public double TEBoom { get; set; }
        public double TEBust { get; set; }
        public double DSTBoom { get; set; }
        public double DSTBust { get; set; }
        public double KBoom { get; set; }
        public double KBust { get; set; }
    }

    public class GeoDistance
    {
        public int RadiusMiles { get; set; }
        public int LongLatDec { get; set; }
    }

    public class FiveThirtyEightValueSettings
    {
        public double PAtts { get; set; }
        public double PComps { get; set; }
        public double PTds { get; set; }
        public double PYds { get; set; }
        public double Ints { get; set; }
        public double Sacks { get; set; }
        public double RAtts { get; set; }
        public double RYds { get; set; }
        public double RTds { get; set; }
    }

    public class StartOrSitSettings
    {
        public double QBCompareRange { get; set; }
        public double RBCompareRange { get; set; }
        public double WRCompareRange { get; set; }
        public double TECompareRange { get; set; }
    }

    public class SleeperSettings
    {
        public string SleeperBaseURL { get; set; } = "";
    }

    public class WeeklyScraping
    {
        public string FantasyProsBaseURL { get; set; } = "";
        public string NFLTeamLogoURL { get; set; } = "";
        public string FantasyProsXPath { get; set; } = "";
        public string UploadScheduleURL { get; set; } = "";
        public string UploadScheduleXPath { get; set; } = "";
        public string RedZoneURL { get; set; } = "";
        public string RedZoneXPath { get; set; } = "";
        public string SnapCountURL { get; set; } = "";
        public string SnapCountXPath { get; set; } = "";
    }

    public class WeatherAPI
    {
        public string WeatherAPIKey { get; set; } = "";
        public string WeatherAPIURL { get; set; } = "";
        public string ForecastDays { get; set; } = "";
    }

    public class JsonOptions
    {
        public JsonSerializerOptions Options { get; }

        public JsonOptions()
        {
            Options = new JsonSerializerOptions{

                PropertyNameCaseInsensitive = true
            };
        }
    }
       
}
