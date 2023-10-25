using Football.Fantasy.MockDraft.Models;
using Football.Players.Models;
using Football.Enums;
using Football.Models;
using Football.Data.Models;

namespace Football.Fantasy.MockDraft.Interfaces
{
    public interface IMockDraftService
    {
        public Task<List<Position>> GetTeamNeeds(FantasyTeam team);
        public Task<List<SeasonADP>> GetPlayerChoices(List<Position> teamNeeds, List<SeasonADP> availablePlayers);
        public Task<SeasonADP> ChoosePlayer(List<SeasonADP> players);
        public Task<List<SeasonADP>> GetAvailablePlayers(Mock mock);
        public Task<int> AddPlayerToTeam(MockDraftResults result);
        public Task<Mock> CreateMockDraft(MockDraftParameters param);
        public Task<int> CreateFantasyTeam(int mockDraftId, int draftPosition, string teamName);
        public Task<List<FantasyTeam>> PopulateFantasyTeams(int mockDraftId, int userPosition, int teamCount, string teamName);
    }
}
