using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;

namespace Football.Players.Services
{
    public class TeamsService(IStatisticsService statisticsService, IPlayersService playersService, ITeamsRepository teamsRepository, IOptionsMonitor<Season> season) : ITeamsService
    {
        private readonly Season _season = season.CurrentValue;
        public async Task<IEnumerable<TeamLeagueInformation>> GetTeamsInDivision(int teamId) => await teamsRepository.GetTeamsLeagueInformationByDivision((await GetTeamLeagueInformation(teamId)).Division, teamId);
        public async Task<IEnumerable<TeamLeagueInformation>> GetTeamsByDivision(Division division) => await teamsRepository.GetTeamsLeagueInformationByDivision(division.ToString());
        public async Task<IEnumerable<TeamLeagueInformation>> GetTeamsByConference(Conference conference) => await teamsRepository.GetTeamLeagueInformationByConference(conference.ToString());
        public async Task<TeamLeagueInformation> GetTeamLeagueInformation(int teamId) => await teamsRepository.GetTeamLeagueInformation(teamId);

        public async Task<TeamRecord> GetTeamRecordInDivision(int teamId)
        {
            var gameResults = await statisticsService.GetGameResults(_season.CurrentSeason);
            var teamsInDivision = (await GetTeamsInDivision(teamId)).Select(t => t.TeamId).ToList();
            var teamsDictionary = (await playersService.GetAllTeams()).ToDictionary(t => t.TeamId);
            var gamesWon = gameResults.Where(g => (g.WinnerId == teamId && teamsInDivision.Contains(g.LoserId)));
            var gamesLost = gameResults.Where(g => (g.LoserId == teamId && teamsInDivision.Contains(g.WinnerId)));
            var gamesTied = gameResults.Where(g => (g.LoserPoints == g.WinnerPoints && ((g.HomeTeamId == teamId || g.AwayTeamId == teamId)) && (teamsInDivision.Contains(g.HomeTeamId) || teamsInDivision.Contains(g.AwayTeamId))));
            return new TeamRecord
            {
                TeamMap = teamsDictionary[teamId],
                Wins = gamesWon.Count(),
                Losses = gamesLost.Count(),
                Ties = gamesTied.Count()
            };
        }
    }
}
