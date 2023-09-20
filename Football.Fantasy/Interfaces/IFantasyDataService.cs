using Football.Data.Models;
using Football.Fantasy.Models;
using Football.Players.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyDataService
    {
        public Task<SeasonFantasy> GetSeasonFantasy(int playerId, int season);
        public Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId);
        public Task<int> PostSeasonFantasy(int season, string position);
        public Task<int> PostWeeklyFantasy(int season, int week, string position);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week);
        public Task<List<SeasonFantasy>> GetCurrentFantasyTotals(int season);
    }
}
