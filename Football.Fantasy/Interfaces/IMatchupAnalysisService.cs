using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMatchupAnalysisService
    {
        public Task<List<MatchupRanking>> GetPositionalMatchupRankingsFromSQL(Position position, int season, int week);
        public Task<int> PostMatchupRankings(Position position, int week = 0);
        public Task<int> GetMatchupRanking(int playerId);
        public Task<List<WeeklyFantasy>> GetTopOpponents(Position position, int teamId);
        
    }
}
