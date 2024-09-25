
namespace Football.Shared.Models.Players
{
    public class PlayerTeamModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public int Season { get; set; }
        public string Team { get; set; } = "";
        public int TeamId { get; set; }
    }
}
