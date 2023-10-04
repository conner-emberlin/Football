namespace Football.Data.Models
{
    public class SeasonDataQB : FantasyProsStringParseQB
    {
        public int Season { get; set; }
        public int PlayerId { get; set; }
    }
    public class SeasonDataRB : FantasyProsStringParseRB
    {
        public int Season { get; set; }
        public int PlayerId { get; set; }
    }
    public class SeasonDataWR : FantasyProsStringParseWR
    {
        public int Season { get; set; }
        public int PlayerId { get; set; }

    }
    public class SeasonDataTE : FantasyProsStringParseTE
    {
        public int Season { get; set; }
        public int PlayerId { get; set; }
    }
    public class SeasonDataDST : FantasyProsStringParseDST
    {
        public int Season { get; set; }
        public int PlayerId { get; set; }
    }
    public class SeasonADP : FantasyProsADP
    {
        public int Season { get; set;}
        public int PlayerId { get; set; }
        public string Position { get; set; } = "";
    }

}
