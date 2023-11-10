using Football.Enums;
using Football.Fantasy.Analysis.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IFantasyAnalysisService
    {
        public Task<List<BoomBust>> GetBoomBusts(Position position);
        public Task<List<FantasyPerformance>> GetFantasyPerformances(Position position);
        public Task<List<BoomBustByWeek>> GetBoomBustsByWeek(int playerId);
        public Task<List<FantasyPercentage>> GetFantasyPercentages(Position position);
    }
}
