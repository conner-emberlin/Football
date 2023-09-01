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



}
