namespace Football.Data.Models
{
    public class FantasyProsStringParseWR
    {
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

    public class FantasyProsStringParseQB
    {
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

    public class FantasyProsStringParseRB
    {
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
    public class FantasyProsStringParseTE
    {
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
    public class FantasyProsStringParseDST
    {
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

    public class FantasyProsADP
    {
        public string Name { get; set; } = "";
        public double PositionADP { get; set; }
        public double OverallADP { get; set; }
    }

    public class FantasyProsRosterPercent
    {
        public string Name { get; set; } = "";
        public double RosterPercent { get; set; }
    }




}
