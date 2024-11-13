using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface ITeamsRepository
    {
        public Task<TeamLeagueInformation> GetTeamLeagueInformation(int teamId);
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamsLeagueInformationByDivision(string division, int teamId = 0);
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamLeagueInformationByConference(string conference);
        public Task<int> GetTeamId(string teamName);
        public Task<int> GetTeamId(int playerId);
        public Task<int> GetTeamIdFromDescription(string teamDescription);
        public Task<List<TeamMap>> GetAllTeams();
        public Task<TeamMap> GetTeam(int teamId);
        public Task<PlayerTeam?> GetPlayerTeam(int season, int playerId);
        public Task<IEnumerable<PlayerTeam>> GetPlayerTeams(int season, IEnumerable<int> playerIds);
        public Task<List<PlayerTeam>> GetPlayersByTeam(string team, int season);
        public Task<IEnumerable<PlayerTeam>> GetPlayersByTeamIdAndPosition(int teamId, string position, int season, bool activeOnly = false);
        public Task<List<Schedule>> GetUpcomingGames(int teamId, int season, int currentWeek);
        public Task<IEnumerable<Schedule>> GetByeWeeks(int season);
        public Task<List<Schedule>> GetTeamGames(int teamId, int season);
        public Task<TeamLocation> GetTeamLocation(int teamId);
        public Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week);
        public Task<IEnumerable<Schedule>> GetWeeklySchedule(int season, int week);
    }
}
