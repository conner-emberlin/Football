using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMatchupAnalysisRepository
    {
        public Task<int> PostMatchupRankings(List<MatchupRanking> rankings);
        public Task<List<MatchupRanking>> GetPositionalMatchupRankingsFromSQL(string position, int season, int week);
    }
}
