using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyDataRepository
    {
        public Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId);
        public Task<int> PostSeasonFantasy(List<SeasonFantasy> data);
        public Task<int> PostWeeklyFantasy(List<WeeklyFantasy> data);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasyByPlayer(int playerId, int season);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week);

    }
}
