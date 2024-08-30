namespace Football.Shared.Models.Fantasy
{
    public class FantasySplitModel
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double FirstHalfPPG { get; set; }
        public double SecondHalfPPG { get; set; }
        public bool DownTrend { get; set; }
    }
}
