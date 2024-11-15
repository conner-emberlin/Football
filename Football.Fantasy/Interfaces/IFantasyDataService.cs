using Football.Enums;
using Football.Fantasy.Models;


namespace Football.Fantasy.Interfaces
{
    public interface IFantasyDataService
    {
        Task<List<SeasonFantasy>> GetSeasonFantasy(int filter, bool isPlayer = true);
        Task<int> PostSeasonFantasy(int season, Position position);
        Task<int> PostWeeklyFantasy(int season, int week, Position position);
        Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId);
        Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId, string team);
        Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week);
        Task<List<WeeklyFantasy>> GetWeeklyFantasy(Position position, int season = 0);
        Task<List<WeeklyFantasy>> GetWeeklyFantasyBySeason(int playerId, int season);
        Task<List<WeeklyFantasy>> GetAllWeeklyFantasyByPosition(Position position);
        Task<List<SeasonFantasy>> GetAllSeasonFantasyByPosition(Position position, int minGames);
        Task<List<WeeklyFantasy>> GetWeeklyTeamFantasy(string team, int week);
        Task<List<SeasonFantasy>> GetCurrentFantasyTotals(int season);
        Task<double> GetAverageSeasonFantasyTotal(int playerId);
        Task<double> GetRecentSeasonFantasyTotal(int playerId);
        Task<Dictionary<int, double>> GetAverageWeeklyFantasyPoints(IEnumerable<int> playerIds, int season);
    }
}
