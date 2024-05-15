using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface IPlayersRepository
    {
        public Task<int> GetPlayerId(string name);
        public Task<int> CreatePlayer(string Name, int Active, string Position);
        public Task<List<Player>> GetPlayersByPosition(string position);
        public Task<List<Player>> GetAllPlayers();
        public Task<Player> GetPlayer(int playerId);
        public Task<Player?> GetPlayerByName(string name);
        public Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position);
        public Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position);
        public Task<List<InjuryConcerns>> GetPlayerInjuries(int season);
        public Task<List<Suspensions>> GetPlayerSuspensions(int season);
        public Task<List<QuarterbackChange>> GetQuarterbackChanges(int season);
        public Task<double> GetEPA(int playerId, int season);
        public Task<double> GetSeasonProjection(int season, int playerId);
        public Task<double> GetWeeklyProjection(int season, int week, int playerId);
        public Task<PlayerTeam?> GetPlayerTeam(int season, int playerId);
        public Task<IEnumerable<PlayerTeam>> GetPlayerTeams(int season, IEnumerable<int> playerIds);
        public Task<List<PlayerTeam>> GetPlayersByTeam(string team, int season);
        public Task<int> GetCurrentWeek(int season);
        public Task<int> GetTeamId(string teamName);
        public Task<int> GetTeamId(int playerId);
        public Task<int> GetTeamIdFromDescription(string teamDescription);
        public Task<List<TeamMap>> GetAllTeams();
        public Task<TeamMap> GetTeam(int teamId);
        public Task<List<Schedule>> GetUpcomingGames(int teamId, int season, int currentWeek);
        public Task<List<Schedule>> GetGames(int season, int week);
        public Task<List<Schedule>> GetTeamGames(int teamId, int season);
        public Task<List<int>> GetIgnoreList();
        public Task<TeamLocation> GetTeamLocation(int teamId);
        public Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week);
        public Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season);
        public Task<int> PostInSeasonInjury(InSeasonInjury injury);
        public Task<bool> UpdateInjury(InSeasonInjury injury);
        public Task<List<InSeasonTeamChange>> GetInSeasonTeamChanges(int season);
        public Task<bool> UpdateCurrentTeam(int playerId, string newTeam, int season);
        public Task<int> PostTeamChange(InSeasonTeamChange teamChange);
        public Task<int> InactivatePlayers(List<int> playerIds);
        public Task<List<int>> GetSeasons();
        public Task<bool> CreateRookie(Rookie rookie);
        public Task<List<Rookie>> GetAllRookies();
    }
}
