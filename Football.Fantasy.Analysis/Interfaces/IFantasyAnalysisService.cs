using Football.Enums;
using Football.Fantasy.Analysis.Models;
using Football.Players.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IFantasyAnalysisService
    {
        public Task<List<BoomBust>> GetBoomBusts(Position position);
        public Task<List<FantasyPerformance>> GetFantasyPerformances(Position position);
        public Task<List<FantasyPerformance>> GetFantasyPerformances(int teamId);
        public Task<FantasyPerformance?> GetFantasyPerformance(Player player);
        public Task<List<BoomBustByWeek>> GetBoomBustsByWeek(int playerId);
        public Task<List<FantasyPercentage>> GetFantasyPercentages(Position position);
        
    }
}
