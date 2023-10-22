using Football.Enums;
using Football.Fantasy.Models;


namespace Football.Fantasy.Interfaces
{
    public interface IFantasyDataService
    {
        public Task<SeasonFantasy> GetSeasonFantasy(int playerId, int season);
        public Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId);
        public Task<int> PostSeasonFantasy(int season, PositionEnum position);
        public Task<int> PostWeeklyFantasy(int season, int week, PositionEnum position);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(PositionEnum position);
        public Task<List<SeasonFantasy>> GetCurrentFantasyTotals(int season);
    }
}
