namespace Football.Players.Models
{
    public class WeeklyDataQB 
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public double Completions { get; set; }
        public double Attempts { get; set; }
        public double Yards { get; set; }
        public double TD { get; set; }
        public double Int { get; set; }
        public double Sacks { get; set; }
        public double RushingAttempts { get; set; }
        public double RushingYards { get; set; }
        public double RushingTD { get; set; }
        public double Fumbles { get; set; }
        public double Games { get; set; }
    }
    public class WeeklyDataRB 
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public double RushingAtt { get; set; }
        public double RushingYds { get; set; }
        public double RushingTD { get; set; }
        public double Receptions { get; set; }
        public double Targets { get; set; }
        public double Yards { get; set; }
        public double ReceivingTD { get; set; }
        public double Fumbles { get; set; }
        public double Games { get; set; }
    }
    public class WeeklyDataWR 
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public double Receptions { get; set; }
        public double Targets { get; set; }
        public double Yards { get; set; }
        public double Long { get; set; }
        public double TD { get; set; }
        public double RushingAtt { get; set; }
        public double RushingYds { get; set; }
        public double RushingTD { get; set; }
        public double Fumbles { get; set; }
        public double Games { get; set; }

    }
    public class WeeklyDataTE
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public double Receptions { get; set; }
        public double Targets { get; set; }
        public double Yards { get; set; }
        public double Long { get; set; }
        public double TD { get; set; }
        public double RushingAtt { get; set; }
        public double RushingYds { get; set; }
        public double RushingTD { get; set; }
        public double Fumbles { get; set; }
        public double Games { get; set; }
    }
    public class WeeklyDataDST 
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public double Sacks { get; set; }
        public double Ints { get; set; }
        public double FumblesRecovered { get; set; }
        public double ForcedFumbles { get; set; }
        public double DefensiveTD { get; set; }
        public double Safties { get; set; }
        public double SpecialTD { get; set; }
        public double Games { get; set; }
    }

    public class WeeklyDataK 
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public double FieldGoals { get; set; }
        public double FieldGoalAttempts { get; set; }
        public double OneNineteen { get; set; }
        public double TwentyTwentyNine { get; set; }
        public double ThirtyThirtyNine { get; set; }
        public double FourtyFourtyNine { get; set; }
        public double Fifty { get; set; }
        public double ExtraPoints { get; set; }
        public double ExtraPointAttempts { get; set; }
        public double Games { get; set; }
    }
    public class WeeklyRosterPercent 
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public double RosterPercent { get; set; }
    }

    public class GameResult 
    {
        public int Season { get; set; }
        public int WinnerId { get; set; }      
        public int LoserId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int Week { get; set; }
        public string Day { get; set; } = "";
        public string Date { get; set; } = "";
        public string Time { get; set; } = "";
        public string Winner { get; set; } = "";
        public string HomeIndicator { get; set; } = "";
        public string Loser { get; set; } = "";
        public int WinnerPoints { get; set; }
        public int LoserPoints { get; set; }
        public int WinnerYards { get; set; }
        public int LoserYards { get; set; }
    }

    public class SnapCount 
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Position { get; set; } = "";
        public string Name { get; set; } = "";
        public double Snaps { get; set; }
    }

    public class WeeklyRedZoneRB : WeeklyDataRB
    {
        public int Yardline { get; set; }
    }

    public class AllWeeklyDataQB: WeeklyDataQB
    {
        public double Snaps { get; set; }
    }

    public class AllWeeklyDataRB : WeeklyDataRB
    {
        public double Snaps { get; set;}
    }

    public class AllWeeklyDataWR : WeeklyDataWR
    {
        public double Snaps { get; set; }    
    }

    public class AllWeeklyDataTE : WeeklyDataTE
    {
        public double Snaps { get; set; }
    } 

    public class AllWeeklyDataK : WeeklyDataK
    {

    }

    public class AllWeeklyDataDST : WeeklyDataDST
    {

    }

}
