using Football.Enums;
using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface ITeamsService
    {
        Task<TeamLeagueInformation> GetTeamLeagueInformation(int teamId);
        Task<IEnumerable<TeamLeagueInformation>> GetTeamsInDivision(int teamId);
        Task<IEnumerable<TeamLeagueInformation>> GetTeamsByDivision(Division division);
        Task<IEnumerable<TeamLeagueInformation>> GetTeamsByConference(Conference conference);
        Task<int> GetTeamId(string teamName);
        Task<int> GetTeamId(int playerId);
        Task<int> GetTeamIdFromDescription(string teamDescription);
        Task<List<TeamMap>> GetAllTeams();
        Task<TeamMap> GetTeam(int teamId);
        Task<TeamRecord> GetTeamRecordInDivision(int teamId);
        Task<List<TeamRecord>> GetTeamRecords(int season);
        Task<PlayerTeam?> GetPlayerTeam(int season, int playerId);
        Task<IEnumerable<PlayerTeam>> GetPlayerTeams(int season, IEnumerable<int> playerIds);
        Task<List<PlayerTeam>> GetPlayersByTeam(string team);
        Task<IEnumerable<PlayerTeam>> GetPlayersByTeamIdAndPosition(int teamId, Position position, int season, bool activeOnly = false);
        Task<List<Schedule>> GetUpcomingGames(int playerId);
        Task<TeamLocation> GetTeamLocation(int teamId);
        Task<List<Schedule>> GetTeamGames(int teamId);
        Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week);
        Task<IEnumerable<Schedule>> GetWeeklySchedule(int season, int week);
        Task<IEnumerable<Schedule>> GetByeWeeks(int season);
        Task<IEnumerable<TeamDepthChart>> GetTeamDepthChart(int teamId, int season = 0);

    }
}
