using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;

namespace Football.Players.Services
{
    public class StatisticsService(IStatisticsRepository statisticsRepository, IPlayersService playersService, ISettingsService settingsService, IOptionsMonitor<Season> season) : IStatisticsService
    {
        private readonly Season _season = season.CurrentValue;
        public async Task<List<T>> GetSeasonData<T>(Position position, int queryParam, bool isPlayer) => await statisticsRepository.GetSeasonData<T>(position, queryParam, isPlayer);
        public async Task<List<T>> GetWeeklyData<T>(Position position, int playerId) => await statisticsRepository.GetWeeklyDataByPlayer<T>(position, playerId, _season.CurrentSeason);
        public async Task<List<T>> GetWeeklyData<T>(Position position, int season, int week) => await statisticsRepository.GetWeeklyData<T>(position, season, week);
        public async Task<List<T>> GetAllWeeklyDataByPosition<T>(Position position) => await statisticsRepository.GetAllWeeklyDataByPosition<T>(position);
        public async Task<List<T>> GetAllSeasonDataByPosition<T>(Position position) => (await statisticsRepository.GetAllSeasonDataByPosition<T>(position));
        public async Task<List<GameResult>> GetGameResults(int season) => await statisticsRepository.GetGameResults(season);
        public async Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week) => await statisticsRepository.GetWeeklyRosterPercentages(season, week);
        public async Task<List<SnapCount>> GetSnapCounts(int playerId) => await statisticsRepository.GetSnapCounts(playerId, _season.CurrentSeason);
        public async Task<double> GetSnapsByGame(int playerId, int season, int week) => await statisticsRepository.GetSnapsByGame(playerId, season, week);
        public async Task<List<T>> GetSeasonDataByTeamIdAndPosition<T>(int teamId, Position position, int season) => await statisticsRepository.GetSeasonDataByTeamIdAndPosition<T>(teamId, position, season);
        public async Task<double> GetYearsExperience(int playerId, Position position) => await statisticsRepository.GetYearsExperience(playerId, position);
        public async Task<IEnumerable<StarterMissedGames>> GetCurrentStartersThatMissedGamesLastSeason(int currentSeason, int previousSeason, int maxGames, double avgProjection) => await statisticsRepository.GetCurrentStartersThatMissedGamesLastSeason(currentSeason, previousSeason, maxGames, avgProjection);
        public async Task<List<TeamRecord>> GetTeamRecords(int season)
        {
            var gameResults = await GetGameResults(season);
            var teams = await playersService.GetAllTeams();
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

        public async Task<List<T>> GetWeeklyData<T>(Position position, int playerId, string team)
        {
            var weeklyData = await statisticsRepository.GetWeeklyDataByPlayer<T>(position, playerId, _season.CurrentSeason);
            var teamChanges = await playersService.GetInSeasonTeamChanges();
            if (teamChanges.Any(t => t.PlayerId == playerId))
            {
                List<T> filteredData = [];
                var teamChange = teamChanges.First(t => t.PlayerId == playerId);

                foreach (var data in weeklyData)
                {
                    var week = (int)settingsService.GetValueFromModel(data, Model.Week);
                    if (teamChange.PreviousTeam == team && week < teamChange.WeekEffective)
                        filteredData.Add(data);
                    else if(teamChange.NewTeam == team && week >= teamChange.WeekEffective)
                        filteredData.Add(data);
                }
                return filteredData;
            }
            else return weeklyData;
        }




    }
}
