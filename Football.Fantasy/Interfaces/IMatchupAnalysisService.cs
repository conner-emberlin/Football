using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMatchupAnalysisService
    {
        Task<List<MatchupRanking>> GetPositionalMatchupRankingsFromSQL(Position position, int season, int week);
        Task<int> PostMatchupRankings(Position position, int week = 0);
        Task<int> GetMatchupRanking(int playerId);
        Task<List<WeeklyFantasy>> GetTopOpponents(Position position, int teamId);
        Task<IEnumerable<MatchupRanking>> GetRestOfSeasonMatchupRankingsByTeam(int teamId);

    }
}
