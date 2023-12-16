namespace Football.Data.Models
{
    public class WeeklyDataQB : FantasyProsStringParseQB
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
    }
    public class WeeklyDataRB : FantasyProsStringParseRB
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
    }
    public class WeeklyDataWR : FantasyProsStringParseWR
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }

    }
    public class WeeklyDataTE : FantasyProsStringParseTE
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
    }
    public class WeeklyDataDST : FantasyProsStringParseDST
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
    }

    public class WeeklyDataK : FantasyProsStringParseK
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
    }
    public class WeeklyRosterPercent : FantasyProsRosterPercent
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
    }

    public class GameResult : ProFootballReferenceGameScores
    {
        public int Season { get; set; }
        public int WinnerId { get; set; }      
        public int LoserId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
    }

    public class SnapCount : FantasyProsSnapCount
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Position { get; set; } = "";
    }

    public class WeeklyRedZoneRB : WeeklyDataRB
    {
        public int Yardline { get; set; }
    }

}
