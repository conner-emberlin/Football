namespace Football.Api.Models.Fantasy
{
    public class WaiverWireCandidateModel
    {
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public string Team { get; set; } = "";
        public double RosteredPercentage { get; set; }
    }
}
