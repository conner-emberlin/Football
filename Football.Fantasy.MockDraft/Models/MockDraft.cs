using Football.Players.Models;

namespace Football.Fantasy.MockDraft.Models
{
    public class Mock
    {
        public int MockDraftId { get; set; }
        public List<FantasyTeam> Teams { get; set; } = new();
        public int UserPosition { get; set; }

    }

    public class FantasyTeam
    {
        public int FantasyTeamId { get; set; }
        public string FantasyTeamName { get; set; } = "";
        public int DraftPosition { get; set; }
        public List<Player> Players { get; set; } = new();
    }

    public class MockDraftResults
    {
        public int MockDraftId { get; set; }
        public int Round { get; set; }
        public int FantasyTeamId { get; set; }
        public int Pick { get; set; }
        public int PlayerId { get; set; }
    }

    public class MockDraftParameters
    {
        public int TeamCount { get; set; }
        public int UserPosition { get; set; }
        public string TeamName { get; set; } = "";
    }
     
}
