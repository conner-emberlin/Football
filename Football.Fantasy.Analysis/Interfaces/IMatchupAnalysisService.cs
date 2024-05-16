using Football.Enums;
using Football.Fantasy.Analysis.Models;
using Football.Fantasy.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IMatchupAnalysisService
    {
        public Task<List<MatchupRanking>> PositionalMatchupRankings(Position position);
        public Task<int> GetMatchupRanking(int playerId);
        public Task<List<WeeklyFantasy>> GetTopOpponents(Position position, int teamId);
        public Task<int> PostMatchupRankings(Position position);
    }
}
