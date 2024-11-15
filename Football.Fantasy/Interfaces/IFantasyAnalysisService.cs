using Football.Enums;
using Football.Fantasy.Models;
using Football.Players.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyAnalysisService
    {
        Task<List<QualityStarts>> GetQualityStartsByPosition(Position position);
        Task<List<BoomBust>> GetBoomBusts(Position position);
        Task<List<FantasyPerformance>> GetFantasyPerformances(Position position);
        Task<List<FantasyPerformance>> GetFantasyPerformances(int teamId);
        Task<FantasyPerformance?> GetFantasyPerformance(Player player);
        Task<List<BoomBustByWeek>> GetBoomBustsByWeek(int playerId);
        Task<List<FantasyPercentage>> GetFantasyPercentages(Position position);
        Task<FantasyPercentage> GetQBSeasonFantasyPercentageByPlayerId(int season, int playerId);
        Task<FantasyPercentage> GetRBSeasonFantasyPercentageByPlayerId(int season, int playerId);
        Task<List<FantasySplit>?> GetFantasySplits(Position position, int season);
        Task<List<WeeklyFantasyTrend>> GetWeeklyFantasyTrendsByPosition(Position position, int season);
        Task<IEnumerable<WeeklyFantasyTrend>> GetPlayerWeeklyFantasyTrends(int playerId, int season);
        
    }
}
