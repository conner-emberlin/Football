using Football.Statistics.Models;

namespace Football.Statistics.Interfaces
{
    public interface IAdvancedStatisticsService
    {
        public Task<List<AdvancedQBStatistics>> GetAdvancedQBStatistics();
        public Task<double> FiveThirtyEightQBValue(int playerId);
        public Task<double> PasserRating(int playerId);
        public Task<double> QBYardsPerPlay(int playerId);
        
    }
}
