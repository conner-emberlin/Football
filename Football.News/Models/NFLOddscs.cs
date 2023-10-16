namespace Football.News.Models
{
    public class NFLOddsAPI
    {
        public string NFLOddsAPIKey { get; set; } = "";
        public string NFLOddsAPIURL { get; set; } = "";
        public string DefaultBookmaker { get; set; } = "";
    }
    public class Bookmaker
    {
        public string key { get; set; }
        public string title { get; set; }
        public DateTime last_update { get; set; }
        public List<Market> markets { get; set; }
    }

    public class Market
    {
        public string key { get; set; }
        public DateTime last_update { get; set; }
        public List<Outcome> outcomes { get; set; }
    }

    public class Outcome
    {
        public string name { get; set; }
        public int price { get; set; }
        public double? point { get; set; }
    }

    public class NFLOddsRoot
    {
        public string id { get; set; }
        public string sport_key { get; set; }
        public string sport_title { get; set; }
        public DateTime commence_time { get; set; }
        public string home_team { get; set; }
        public string away_team { get; set; }
        public List<Bookmaker> bookmakers { get; set; }
    }


}
