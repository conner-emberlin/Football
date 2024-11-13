using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Players.Repository;
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
        public async Task<List<TeamMap>> GetAllTeams() => await teamsRepository.GetAllTeams();
        public async Task<TeamMap> GetTeam(int teamId) => await teamsRepository.GetTeam(teamId);
        public async Task<int> GetTeamId(string teamName) => await teamsRepository.GetTeamId(teamName);
        public async Task<int> GetTeamId(int playerId) => await teamsRepository.GetTeamId(playerId);
        public async Task<IEnumerable<PlayerTeam>> GetPlayerTeams(int season, IEnumerable<int> playerIds) => await teamsRepository.GetPlayerTeams(season, playerIds);
        public async Task<int> GetTeamIdFromDescription(string teamDescription) => await teamsRepository.GetTeamIdFromDescription(teamDescription);
        public async Task<TeamRecord> GetTeamRecordInDivision(int teamId)
        {
            var gameResults = await statisticsService.GetGameResults(_season.CurrentSeason);
            var teamsInDivision = (await GetTeamsInDivision(teamId)).Select(t => t.TeamId).ToList();
            var teamsDictionary = (await GetAllTeams()).ToDictionary(t => t.TeamId);
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
        public async Task<List<TeamRecord>> GetTeamRecords(int season)
        {
            var gameResults = await statisticsService.GetGameResults(season);
            var teams = await GetAllTeams();
            var currentWeek = await playersService.GetCurrentWeek(season);
            return teams.Select(team => new TeamRecord
            {
                TeamMap = team,
                Season = season,
                CurrentWeek = currentWeek,
                Wins = gameResults.Count(gr => gr.WinnerId == team.TeamId),
                Losses = gameResults.Count(gr => gr.LoserId == team.TeamId),
                Ties = gameResults.Count(gr => gr.LoserPoints == gr.WinnerPoints
                                            && (gr.HomeTeamId == team.TeamId || gr.AwayTeamId == team.TeamId))
            }).ToList();
        }
        public async Task<IEnumerable<PlayerTeam>> GetPlayersByTeamIdAndPosition(int teamId, Position position, int season, bool activeOnly = false) => await teamsRepository.GetPlayersByTeamIdAndPosition(teamId, position.ToString(), season, activeOnly);
        public async Task<List<PlayerTeam>> GetPlayersByTeam(string team)
        {
            var playerTeams = await teamsRepository.GetPlayersByTeam(team, _season.CurrentSeason);
            var teamId = await GetTeamId(team);
            var teamMap = await GetTeam(teamId);
            playerTeams.Add(new PlayerTeam
            {
                PlayerId = teamMap.PlayerId,
                Name = teamMap.TeamDescription,
                Season = _season.CurrentSeason,
                Team = team,
                TeamId = teamId
            });
            var formerPlayers = (await playersService.GetInSeasonTeamChanges()).Where(t => t.PreviousTeam == team);
            if (formerPlayers.Any())
            {
                foreach (var formerPlayer in formerPlayers)
                {
                    var player = await playersService.GetPlayer(formerPlayer.PlayerId);
                    playerTeams.Add(new PlayerTeam
                    {
                        PlayerId = player.PlayerId,
                        Name = player.Name,
                        Season = _season.CurrentSeason,
                        Team = team,
                        TeamId = teamMap.TeamId
                    });
                }
            }
            return playerTeams;
        }
        public async Task<PlayerTeam?> GetPlayerTeam(int season, int playerId)
        {
            var player = await playersService.GetPlayer(playerId);
            if (player.Position == Position.DST.ToString())
            {
                var teamId = await GetTeamId(playerId);
                var team = await GetTeam(teamId);
                return new PlayerTeam
                {
                    PlayerId = playerId,
                    Name = team.TeamDescription,
                    Season = season,
                    Team = team.Team,
                    TeamId = teamId
                };
            }
            else return await teamsRepository.GetPlayerTeam(season, playerId);
        }
        public async Task<List<Schedule>> GetUpcomingGames(int playerId)
        {
            var team = await GetPlayerTeam(_season.CurrentSeason, playerId);
            return team != null ? await teamsRepository.GetUpcomingGames(await GetTeamId(team.Team), _season.CurrentSeason, await playersService.GetCurrentWeek(_season.CurrentSeason)) : [];
        }
    }
}
