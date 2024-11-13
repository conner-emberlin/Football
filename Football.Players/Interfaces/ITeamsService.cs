using Football.Enums;
using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface ITeamsService
    {
        public Task<TeamLeagueInformation> GetTeamLeagueInformation(int teamId);
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamsInDivision(int teamId);
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamsByDivision(Division division);
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamsByConference(Conference conference);
        public Task<int> GetTeamId(string teamName);
        public Task<int> GetTeamId(int playerId);
        public Task<int> GetTeamIdFromDescription(string teamDescription);
        public Task<List<TeamMap>> GetAllTeams();
        public Task<TeamMap> GetTeam(int teamId);
        public Task<TeamRecord> GetTeamRecordInDivision(int teamId);
        public Task<List<TeamRecord>> GetTeamRecords(int season);
        public Task<PlayerTeam?> GetPlayerTeam(int season, int playerId);
        public Task<IEnumerable<PlayerTeam>> GetPlayerTeams(int season, IEnumerable<int> playerIds);
        public Task<List<PlayerTeam>> GetPlayersByTeam(string team);
        public Task<IEnumerable<PlayerTeam>> GetPlayersByTeamIdAndPosition(int teamId, Position position, int season, bool activeOnly = false);
        public Task<List<Schedule>> GetUpcomingGames(int playerId);
        public Task<TeamLocation> GetTeamLocation(int teamId);
        public Task<List<Schedule>> GetTeamGames(int teamId);
        public Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week);
        public Task<IEnumerable<Schedule>> GetWeeklySchedule(int season, int week);
        public Task<IEnumerable<Schedule>> GetByeWeeks(int season);

    }
}
