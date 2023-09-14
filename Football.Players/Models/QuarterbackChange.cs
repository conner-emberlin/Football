namespace Football.Players.Models
{
    public class QuarterbackChange
    {
        public int PlayerId { get; set; }     
        public int Season { get; set; }
        public int PreviousQB { get; set; }
        public int CurrentQB { get; set; }
    }
}
