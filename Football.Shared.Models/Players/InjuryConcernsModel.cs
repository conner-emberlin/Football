namespace Football.Shared.Models.Players
{
    public class InjuryConcernsModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public int Season { get; set; }
        public double Games { get; set; }
        public bool Suspension { get; set; }
        public string Detail { get; set; } = "";
    }
}
