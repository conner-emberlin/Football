namespace Football.Players.Models
{
    public class InjuryConcerns
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double Games { get; set; }
        public string Detail { get; set; } = "";
    }
}
