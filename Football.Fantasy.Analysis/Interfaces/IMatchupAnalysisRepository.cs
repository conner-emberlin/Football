using Football.Fantasy.Analysis.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IMatchupAnalysisRepository
    {
        public Task<int> PostMatchupRankings(List<MatchupRanking> rankings);
    }
}
