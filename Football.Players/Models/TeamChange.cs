

using Football.Enums;

namespace Football.Players.Models
{
    public class TeamChange
    {
        public int PlayerId { get; set; }
        public Position Position { get; set; }
        public int Season { get; set; }
        public int PreviousTeamId { get; set; }
        public int CurrentTeamId { get; set; }

    }
}
