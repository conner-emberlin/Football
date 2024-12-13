using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyDataRepository
    {
        Task<List<SeasonFantasy>> GetSeasonFantasy(int filter, bool isPlayer = true);
        Task<List<SeasonFantasy>> GetAllSeasonFantasyByPosition(string position, int minGames);
        Task<int> PostSeasonFantasy(List<SeasonFantasy> data);
        Task<int> PostWeeklyFantasy(List<WeeklyFantasy> data);
        Task<List<WeeklyFantasy>> GetWeeklyFantasyByPlayer(int playerId, int season);
        Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week);
        Task<List<WeeklyFantasy>> GetAllWeeklyFantasyByPosition(string position, int season = 0);
        Task<IEnumerable<WeeklyFantasy>> GetAllWeeklyFantasy(int season);
        Task<Dictionary<int, double>> GetAverageWeeklyFantasyPoints(IEnumerable<int> playerIds, int season);

    }
}
