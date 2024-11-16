using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMatchupAnalysisRepository
    {
        Task<int> PostMatchupRankings(List<MatchupRanking> rankings);
        Task<List<MatchupRanking>> GetPositionalMatchupRankingsFromSQL(string position, int season, int week);
        Task<List<MatchupRanking>> GetCurrentMatchupRankings(int week);
    }
}
