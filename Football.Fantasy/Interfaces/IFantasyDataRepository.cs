using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyDataRepository
    {
        public Task<List<SeasonFantasy>> GetSeasonFantasy(int filter, bool isPlayer = true);
        public Task<List<SeasonFantasy>> GetAllSeasonFantasyByPosition(string position, int minGames);
        public Task<int> PostSeasonFantasy(List<SeasonFantasy> data);
        public Task<int> PostWeeklyFantasy(List<WeeklyFantasy> data);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasyByPlayer(int playerId, int season);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week);
        public Task<List<WeeklyFantasy>> GetAllWeeklyFantasyByPosition(string position, int season = 0);
        public Task<Dictionary<int, double>> GetAverageWeeklyFantasyPoints(IEnumerable<int> playerIds, int season);

    }
}
