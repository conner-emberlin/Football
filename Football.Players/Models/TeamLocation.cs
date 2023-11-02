namespace Football.Players.Models
{
    public class TeamLocation
    {
        public int TeamId { get; set; }
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string Zip { get; set; } = "";
        public int Indoor { get; set; }
    }
}
