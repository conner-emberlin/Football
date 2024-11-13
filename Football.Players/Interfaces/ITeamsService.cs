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
        public Task<TeamRecord> GetTeamRecordInDivision(int teamId);

    }
}
