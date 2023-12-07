using Football.Statistics.Models;

namespace Football.Statistics.Interfaces
{
    public interface IAdvancedStatisticsService
    {
        public Task<List<AdvancedQBStatistics>> GetAdvancedQBStatistics();
        public Task<double> FiveThirtyEightQBValue(int playerId);
        public Task<double> PasserRating(int playerId);
        public Task<double> QBYardsPerPlay(int playerId);
        public Task<double> YardsAllowedPerGame(int teamId);
        public Task<List<StrengthOfSchedule>> RemainingStrengthOfSchedule();
        public Task<double> StrengthOfSchedule(int teamId, int atWeek);


    }
}
