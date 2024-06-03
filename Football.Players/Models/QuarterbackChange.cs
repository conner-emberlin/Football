namespace Football.Players.Models
{
    public class QuarterbackChange
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int PreviousQBId { get; set; }
        public int CurrentQBId { get; set; }
        public bool CurrentQBIsRookie { get; set; }
    }
}
