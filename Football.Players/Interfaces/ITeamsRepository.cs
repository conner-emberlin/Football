using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface ITeamsRepository
    {
        Task<TeamLeagueInformation> GetTeamLeagueInformation(int teamId);
        Task<IEnumerable<TeamLeagueInformation>> GetTeamsLeagueInformationByDivision(string division, int teamId = 0);
        Task<IEnumerable<TeamLeagueInformation>> GetTeamLeagueInformationByConference(string conference);
        Task<int> GetTeamId(string teamName);
        Task<int> GetTeamId(int playerId);
        Task<int> GetTeamIdFromDescription(string teamDescription);
        Task<List<TeamMap>> GetAllTeams();
        Task<TeamMap> GetTeam(int teamId);
        Task<PlayerTeam?> GetPlayerTeam(int season, int playerId);
        Task<IEnumerable<PlayerTeam>> GetPlayerTeams(int season, IEnumerable<int> playerIds);
        Task<List<PlayerTeam>> GetPlayersByTeam(string team, int season);
        Task<IEnumerable<PlayerTeam>> GetPlayersByTeamIdAndPosition(int teamId, string position, int season, bool activeOnly = false);
        Task<List<Schedule>> GetUpcomingGames(int teamId, int season, int currentWeek);
        Task<IEnumerable<Schedule>> GetByeWeeks(int season);
        Task<List<Schedule>> GetTeamGames(int teamId, int season);
        Task<TeamLocation> GetTeamLocation(int teamId);
        Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week);
        Task<IEnumerable<Schedule>> GetWeeklySchedule(int season, int week);
    }
}
