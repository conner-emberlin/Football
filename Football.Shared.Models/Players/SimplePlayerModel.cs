namespace Football.Shared.Models.Players
{
    public class SimplePlayerModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public int Active { get; set; }
    }
}
