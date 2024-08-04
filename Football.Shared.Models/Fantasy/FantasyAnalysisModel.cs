namespace Football.Shared.Models.Fantasy
{
    public class FantasyAnalysisModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public int Season { get; set; }
        public double TotalFantasy { get; set; }
        public double AvgFantasy { get; set; }
        public double MinFantasy { get; set; }
        public double MaxFantasy { get; set; }
        public double Variance { get; set; }
        public double StdDev { get; set; }
        public double BoomPercentage { get; set; }
        public double BustPercentage { get; set; }


    }
}
