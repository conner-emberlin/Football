using Football.Enums;
using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface IAdvancedStatisticsService
    {
        Task<List<AdvancedQBStatistics>> GetAdvancedQBStatistics();
        Task<double> FiveThirtyEightQBValue(int playerId);
        Task<double> PasserRating(int playerId);
        Task<double> QBYardsPerPlay(int playerId);
        Task<double> YardsAllowedPerGame(int teamId);
        Task<List<StrengthOfSchedule>> RemainingStrengthOfSchedule();
        Task<double> StrengthOfSchedule(int teamId, int atWeek);
        Task<IEnumerable<DivisionStanding>> GetStandingsByDivision(Division division);

    }
}
