namespace Football.Api.Models.Players
{
    public class SnapCountModel
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int PlayerId { get; set; }
        public string Position { get; set; } = "";
        public string Name { get; set; } = "";
        public double Snaps { get; set; }
    }
}
