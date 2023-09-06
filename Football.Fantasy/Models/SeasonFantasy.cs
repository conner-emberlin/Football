namespace Football.Fantasy.Models
{
    public class SeasonFantasy
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double Games { get; set; }
        public double FantasyPoints { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
    }
}
