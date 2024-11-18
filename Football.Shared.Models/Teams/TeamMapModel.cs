namespace Football.Shared.Models.Teams
{
    public class TeamMapModel
    {
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public string Team { get; set; } = "";
        public string TeamDescription { get; set; } = "";
    }
}
