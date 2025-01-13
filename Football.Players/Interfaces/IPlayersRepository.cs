using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface IPlayersRepository
    {
        Task<int> GetPlayerId(string name);
        Task<int> CreatePlayer(string Name, int Active, string Position);
        Task<List<Player>> GetPlayersByPosition(string position, bool activeOnly);
        Task<List<Player>> GetAllPlayers(int active = 0, string position = "");
        Task<Player> GetPlayer(int playerId);
        Task<Player?> GetPlayerByName(string name);
        Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position);
        Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position);
        Task<List<InjuryConcerns>> GetPlayerInjuries(int season);
        Task<List<PlayerInjury>> GetPlayerInjuryHistory(int playerId);
        Task<List<Suspensions>> GetPlayerSuspensions(int season);
        Task<Dictionary<int, double>> GetSeasonProjections(IEnumerable<int> playerIds, int season);
        Task<double> GetWeeklyProjection(int season, int week, int playerId);
        Task<int> GetCurrentWeek(int season);
        Task<List<int>> GetIgnoreList();
        Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season);
        Task<Dictionary<int, double>> GetGamesPlayedInjuredBySeason(int season);
        Task<int> PostInSeasonInjury(InSeasonInjury injury);
        Task<bool> UpdateInjury(InSeasonInjury injury);
        Task<List<InSeasonTeamChange>> GetInSeasonTeamChanges(int season);
        Task<bool> UpdateCurrentTeam(int playerId, string newTeam, int season);
        Task<int> PostTeamChange(InSeasonTeamChange teamChange);
        Task<int> InactivatePlayers(List<int> playerIds);
        Task<int> ActivatePlayer(int playerId);
        Task<List<int>> GetSeasons();
        Task<bool> CreateRookie(Rookie rookie);
        Task<List<Rookie>> GetAllRookies();
        Task<int> UploadSleeperPlayerMap(List<SleeperPlayerMap> playerMap);
        Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId);
        Task<List<SleeperPlayerMap>> GetSleeperPlayerMaps();
        Task<IEnumerable<TeamChange>> GetAllTeamChanges(int currentSeason, int previousSeason);
        Task<SeasonInfo?> GetSeasonInfo(int season);
        Task<bool> PostSeasonInfo(SeasonInfo season);
    }
}
