namespace Football.Players.Models
{
    public class SeasonDataQB 
    {
        public int Season { get; set; }
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
    public class SeasonDataRB 
    {
        public int Season { get; set; }
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
    public class SeasonDataWR 
    {
        public int Season { get; set; }
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
    public class SeasonDataTE 
    {
        public int Season { get; set; }
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
    public class SeasonDataDST 
    {
        public int Season { get; set; }
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
    public class SeasonADP
    {
        public int Season { get; set;}
        public int PlayerId { get; set; }
        public string Position { get; set; } = "";
        public string Name { get; set; } = "";
        public double PositionADP { get; set; }
        public double OverallADP { get; set; }
    }

    public class AllSeasonDataQB: SeasonDataQB
    {
        public double YearsExperience { get; set; }
    }

    public class AllSeasonDataRB: SeasonDataRB
    {
        public double YearsExperience { get; set; }
    }
    public class AllSeasonDataWR: SeasonDataWR
    {
        public double YearsExperience { get; set; }
    }
    public class AllSeasonDataTE : SeasonDataTE
    {
        public double YearsExperience { get; set; }
    } 

}
