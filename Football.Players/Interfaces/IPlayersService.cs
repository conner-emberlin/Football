using Football.Enums;
using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface IPlayersService
    {
        Task<List<Player>> GetAllPlayers(int active = 0, string position = "");
        Task<int> GetPlayerId(string name);             
        Task<List<Player>> GetPlayersByPosition(Position position, bool activeOnly = false);
        Task<Player> GetPlayer(int playerId);
        Task<Player?> GetPlayerByName(string name);
        Task<Player> CreatePlayer(string name, int active, string position);
        Task<Player> RetrievePlayer(string name, Position position, bool activatePlayer = false);
        Task<int> InactivatePlayers(List<int> playerIds);
        Task<int> ActivatePlayer(int playerId);
        Task<List<InjuryConcerns>> GetInjuryConcerns(int season);
        Task<bool> DeleteInjuryConcern(int season, int playerId);
        Task<bool> PostInjuryConcern(InjuryConcerns concern);
        Task<List<PlayerInjury>> GetPlayerInjuries();
        Task<List<PlayerInjury>> GetPlayerInjuryHistory(int playerId);
        Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season);
        Task<Dictionary<int, double>> GetGamesPlayedInjuredBySeason(int season);
        Task<int> PostInSeasonInjury(InSeasonInjury injury);
        Task<bool> UpdateInjury(InSeasonInjury injury);
        Task<List<Suspensions>> GetPlayerSuspensions(int season);
        Task<bool> DeletePlayerSuspension(int season, int playerId);
        Task<bool> PostPlayerSuspension(Suspensions suspension);
        Task<List<InSeasonTeamChange>> GetInSeasonTeamChanges();
        Task<int> PostInSeasonTeamChange(InSeasonTeamChange teamChange);
        Task<Dictionary<int, double>> GetSeasonProjections(IEnumerable<int> playerIds, int season);
        Task<double> GetWeeklyProjection(int season, int week, int playerId);
        Task<int> GetCurrentWeek(int season);
        Task<List<int>> GetIgnoreList();
        Task<List<int>> GetSeasons();
        Task<int> UploadSleeperPlayerMap();
        Task<List<TrendingPlayer>> GetTrendingPlayers();
        Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId);
        Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position);
        Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position);
        Task<bool> CreateRookie(Rookie rookie);
        Task<List<Rookie>> GetAllRookies();
        Task<int> GetCurrentSeasonGames();
        Task<int> GetCurrentSeasonWeeks();
        Task<int> GetGamesBySeason(int season);
        Task<int> GetWeeksBySeason(int season);
        Task<IEnumerable<TeamChange>> GetAllTeamChanges(int currentSeason);
        Task<bool> PostSeasonInfo(SeasonInfo season);
        Task<SeasonInfo?> GetSeasonInfo(int season);
        Task<IEnumerable<BackupQuarterback>> GetBackupQuarterbacks(int season);
        Task<int> PostBackupQuarterbacks(List<BackupQuarterback> backups);
        Task<int> DeleteAllBackupQuarterbacks(int season);
    }
}
