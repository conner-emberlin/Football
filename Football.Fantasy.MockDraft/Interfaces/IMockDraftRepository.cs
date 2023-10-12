using Football.Fantasy.MockDraft.Models;
using Football.Data.Models;
using Football.Players.Models;

namespace Football.Fantasy.MockDraft.Interfaces
{
    public interface IMockDraftRepository
    {
        public Task<List<SeasonADP>> GetAvailablePlayers(Mock mock, int season);
        public Task<int> AddPlayerToTeam(MockDraftResults result);
        public Task<int> CreateMockDraft(int teamCount, int userPosition);
        public Task<int> CreateFantasyTeam(int mockDraftId, int draftPosition, string teamName);
    }
}
