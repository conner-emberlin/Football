using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMatchupAnalysisService
    {
        public Task<List<MatchupRanking>> PositionalMatchupRankings(PositionEnum position);
        public Task<int> GetMatchupRanking(int playerId);
    }
}
