using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface ITeamsRepository
    {
        public Task<TeamLeagueInformation> GetTeamLeagueInformation(int teamId);
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamsLeagueInformationByDivision(string division, int teamId = 0);
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamLeagueInformationByConference(string conference);
    }
}
