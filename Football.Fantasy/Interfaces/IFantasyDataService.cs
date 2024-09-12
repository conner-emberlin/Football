using Football.Enums;
using Football.Fantasy.Models;


namespace Football.Fantasy.Interfaces
{
    public interface IFantasyDataService
    {
        public Task<List<SeasonFantasy>> GetSeasonFantasy(int filter, bool isPlayer = true);
        public Task<int> PostSeasonFantasy(int season, Position position);
        public Task<int> PostWeeklyFantasy(int season, int week, Position position);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId, string team);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasy(Position position, int season = 0);
        public Task<List<WeeklyFantasy>> GetWeeklyFantasyBySeason(int playerId, int season);
        public Task<List<WeeklyFantasy>> GetAllWeeklyFantasyByPosition(Position position);
        public Task<List<SeasonFantasy>> GetAllSeasonFantasyByPosition(Position position, int minGames);
        public Task<List<WeeklyFantasy>> GetWeeklyTeamFantasy(string team, int week);
        public Task<List<SeasonFantasy>> GetCurrentFantasyTotals(int season);
        public Task<double> GetAverageSeasonFantasyTotal(int playerId);
        public Task<double> GetRecentSeasonFantasyTotal(int playerId);
        public Task<Dictionary<int, double>> GetAverageWeeklyFantasyPoints(IEnumerable<int> playerIds, int season);
    }
}
