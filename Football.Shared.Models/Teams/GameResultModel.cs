namespace Football.Shared.Models.Teams
{
    public class GameResultModel
    {
        public int WinnerId { get; set; }
        public int LoserId { get; set; }
        public int Week { get; set; }
        public string Winner { get; set; } = "";
        public string Loser { get; set; } = "";
        public int WinnerPoints { get; set; }
        public int LoserPoints { get; set; }
        public int WinnerYards { get; set; }
        public int LoserYards { get; set; }
    }
}
