namespace Football.Models
{
    public class Change
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public string? PreviousTeam { get; set; }
        public string? NewTeam { get; set; }
    }
}
