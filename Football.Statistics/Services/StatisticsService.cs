using Football.Data.Models;
using Football.Enums;
using Football.Players.Interfaces;
using Football.Statistics.Interfaces;
using Football.Statistics.Models;

namespace Football.Statistics.Services
{
    public class StatisticsService(IStatisticsRepository statisticsRepository, IPlayersService playersService, ISettingsService settingsService) : IStatisticsService
    {
        public async Task<List<T>> GetSeasonData<T>(Position position, int queryParam, bool isPlayer) => await statisticsRepository.GetSeasonData<T>(position, queryParam, isPlayer);
        public async Task<List<T>> GetWeeklyData<T>(Position position, int playerId) => await statisticsRepository.GetWeeklyData<T>(position, playerId);
        public async Task<List<T>> GetWeeklyData<T>(Position position, int season, int week) => await statisticsRepository.GetWeeklyData<T>(position, season, week);
        public async Task<List<GameResult>> GetGameResults(int season) => await statisticsRepository.GetGameResults(season);
        public async Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week) => await statisticsRepository.GetWeeklyRosterPercentages(season, week);

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
            var weeklyData = await statisticsRepository.GetWeeklyData<T>(position, playerId);
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
