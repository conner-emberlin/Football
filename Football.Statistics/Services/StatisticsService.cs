using Football.Data.Models;
using Football.Enums;
using Football.Players.Interfaces;
using Football.Statistics.Interfaces;
using Football.Statistics.Models;

namespace Football.Statistics.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IPlayersService _playersService;
        private readonly ISettingsService _settingsService;
        public StatisticsService(IStatisticsRepository statisticsRepository, IPlayersService playersService, ISettingsService settingsService)
        {
            _statisticsRepository = statisticsRepository;
            _playersService = playersService;
            _settingsService = settingsService;
        }
        public async Task<List<T>> GetSeasonData<T>(Position position, int queryParam, bool isPlayer) => await _statisticsRepository.GetSeasonData<T>(position, queryParam, isPlayer);
        public async Task<List<T>> GetWeeklyData<T>(Position position, int playerId) => await _statisticsRepository.GetWeeklyData<T>(position, playerId);
        public async Task<List<T>> GetWeeklyData<T>(Position position, int season, int week) => await _statisticsRepository.GetWeeklyData<T>(position, season, week);
        public async Task<List<GameResult>> GetGameResults(int season) => await _statisticsRepository.GetGameResults(season);
        public async Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week) => await _statisticsRepository.GetWeeklyRosterPercentages(season, week);

        public async Task<List<TeamRecord>> GetTeamRecords(int season)
        {
            var gameResults = await GetGameResults(season);
            var teams = await _playersService.GetAllTeams();
            var currentWeek = await _playersService.GetCurrentWeek(season);
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
            var weeklyData = await _statisticsRepository.GetWeeklyData<T>(position, playerId);
            var teamChanges = await _playersService.GetInSeasonTeamChanges();
            if (teamChanges.Any(t => t.PlayerId == playerId))
            {
                List<T> filteredData = new();
                var teamChange = teamChanges.First(t => t.PlayerId == playerId);

                foreach (var data in weeklyData)
                {
                    var week = (int)_settingsService.GetValueFromModel(data, Model.Week);
                    if (teamChange.PreviousTeam == team && week < teamChange.WeekEffective)
                    {
                        filteredData.Add(data);
                    }
                    else if(teamChange.NewTeam == team && week >= teamChange.WeekEffective)
                    {
                        filteredData.Add(data);
                    }
                }
                return filteredData;
            }
            else return weeklyData;
        }
        
    }
}
