using Football.Enums;
using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface IPlayersService
    {
        public Task<int> GetPlayerId(string name);
        public Task<int> CreatePlayer(Player player);
        public Task<List<Player>> GetAllPlayers();
        public Task<List<Player>> GetPlayersByPosition(PositionEnum position);
        public Task<Player> GetPlayer(int playerId);
        public Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position);
        public Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position);
        public Task<int> GetPlayerInjuries(int playerId, int season);
        public Task<int> GetPlayerSuspensions(int playerId, int season);
        public Task<List<QuarterbackChange>> GetQuarterbackChanges(int season);
        public Task<double> GetEPA(int playerId, int season);
        public Task<double> GetSeasonProjection(int season, int playerId);
        public Task<double> GetWeeklyProjection(int season, int week, int playerId);
        public Task<PlayerTeam?> GetPlayerTeam(int season, int playerId);
        public Task<List<PlayerTeam>> GetPlayersByTeam(string team);
        public Task<int> GetTeamId(string teamName);
        public Task<int> GetTeamId(int playerId);
        public Task<int> GetTeamIdFromDescription(string teamDescription);
        public Task<List<TeamMap>> GetAllTeams();
        public Task<TeamMap> GetTeam(int teamId);   
        public Task<int> GetCurrentWeek(int season);
        public Task<List<Schedule>> GetUpcomingGames(int playerId);
        public Task<List<Schedule>> GetGames(int season, int week);
        public Task<List<Schedule>> GetTeamGames(int teamId);
        public Task<List<int>> GetIgnoreList();
        public Task<TeamLocation> GetTeamLocation(int teamId);
        public Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week);  

        public Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season);
        public Task<int> PostInSeasonInjury(InSeasonInjury injury);
    }
}
