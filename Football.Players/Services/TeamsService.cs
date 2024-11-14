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
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamsByDivision(Division division) => teamsRepository.GetTeamsLeagueInformationByDivision(division.ToString());
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamsByConference(Conference conference) => teamsRepository.GetTeamLeagueInformationByConference(conference.ToString());
        public Task<TeamLeagueInformation> GetTeamLeagueInformation(int teamId) => teamsRepository.GetTeamLeagueInformation(teamId);
        public Task<List<TeamMap>> GetAllTeams() => teamsRepository.GetAllTeams();
        public Task<TeamMap> GetTeam(int teamId) => teamsRepository.GetTeam(teamId);
        public Task<int> GetTeamId(string teamName) => teamsRepository.GetTeamId(teamName);
        public Task<int> GetTeamId(int playerId) => teamsRepository.GetTeamId(playerId);
        public Task<IEnumerable<PlayerTeam>> GetPlayerTeams(int season, IEnumerable<int> playerIds) => teamsRepository.GetPlayerTeams(season, playerIds);
        public Task<int> GetTeamIdFromDescription(string teamDescription) => teamsRepository.GetTeamIdFromDescription(teamDescription);
        public Task<List<Schedule>> GetTeamGames(int teamId) => teamsRepository.GetTeamGames(teamId, _season.CurrentSeason);
        public Task<TeamLocation> GetTeamLocation(int teamId) => teamsRepository.GetTeamLocation(teamId);
        public Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week) => teamsRepository.GetScheduleDetails(season, week);
        public Task<IEnumerable<Schedule>> GetByeWeeks(int season) => teamsRepository.GetByeWeeks(season);
        public Task<IEnumerable<Schedule>> GetWeeklySchedule(int season, int week) => teamsRepository.GetWeeklySchedule(season, week);
        public Task<IEnumerable<PlayerTeam>> GetPlayersByTeamIdAndPosition(int teamId, Position position, int season, bool activeOnly = false) => teamsRepository.GetPlayersByTeamIdAndPosition(teamId, position.ToString(), season, activeOnly);
        public async Task<IEnumerable<TeamLeagueInformation>> GetTeamsInDivision(int teamId) => await teamsRepository.GetTeamsLeagueInformationByDivision((await GetTeamLeagueInformation(teamId)).Division, teamId);
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

        public async Task<IEnumerable<TeamDepthChart>> GetTeamDepthChart(int teamId, int season = 0)
        {
            var chartSeason = season > 0 ? season : _season.CurrentSeason;
            var allTeams = (await GetAllTeams()).ToDictionary(t => t.TeamId);
            if (!allTeams.TryGetValue(teamId, out var teamMap)) return [];

            var players = await GetPlayersByTeam(teamMap.Team);
            var snapCounts = await statisticsService.GetSnapCountsBySeason(players.Select(p => p.PlayerId), chartSeason);
            List<TeamDepthChart> depthChart = [];
            foreach (var player in players)
            {
                var playerSnaps = snapCounts.Where(s => s.PlayerId == player.PlayerId);
                var games = playerSnaps.Count();
                depthChart.Add(new TeamDepthChart
                {
                    TeamId = teamId,
                    Team = player.Team,
                    TeamDescription = teamMap.TeamDescription,
                    PlayerId = player.PlayerId,
                    Name = player.Name,
                    Position = games > 0 ? playerSnaps.First().Position : (await playersService.GetPlayer(player.PlayerId)).Position,
                    Games = games,
                    TotalSnaps = games > 0 ? playerSnaps.Sum(s => s.Snaps) : 0,
                    SnapsPerGame = games > 0 ? playerSnaps.Sum(s => s.Snaps)/games : 0
                });
            }
            return depthChart.OrderBy(s => s.Position).ThenByDescending(s => s.TotalSnaps);
        }
    }
}
