using Football.Fantasy.Models;
using Football.Players.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyDataRepository
    {
        public Task<SeasonFantasy> GetSeasonFantasy(int playerId, int season);
        public Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId);
        public Task<int> PostSeasonFantasy(SeasonFantasy data);
        public Task<int> PostWeeklyFantasy(WeeklyFantasy data);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId);

    }
}
