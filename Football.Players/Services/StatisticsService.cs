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
        public Task<List<T>> GetSeasonData<T>(Position position, int queryParam, bool isPlayer) => statisticsRepository.GetSeasonData<T>(position, queryParam, isPlayer);
        public Task<List<T>> GetWeeklyData<T>(Position position, int playerId) => statisticsRepository.GetWeeklyDataByPlayer<T>(position, playerId, _season.CurrentSeason);
        public Task<List<T>> GetWeeklyData<T>(Position position, int season, int week) => statisticsRepository.GetWeeklyData<T>(position, season, week);
        public Task<List<T>> GetWeeklyDataByPlayer<T>(Position position, int playerId, int season) => statisticsRepository.GetWeeklyDataByPlayer<T>(position, playerId, season);
        public Task<List<T>> GetAllWeeklyDataByPosition<T>(Position position) => statisticsRepository.GetAllWeeklyDataByPosition<T>(position);
        public Task<List<T>> GetAllSeasonDataByPosition<T>(Position position) => statisticsRepository.GetAllSeasonDataByPosition<T>(position);
        public Task<List<GameResult>> GetGameResults(int season) => statisticsRepository.GetGameResults(season);
        public Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week) => statisticsRepository.GetWeeklyRosterPercentages(season, week);
        public Task<List<SnapCount>> GetSnapCounts(int playerId) => statisticsRepository.GetSnapCounts(playerId, _season.CurrentSeason);
        public Task<IEnumerable<SnapCount>> GetSnapCountsBySeason(IEnumerable<int> playerIds, int season) => statisticsRepository.GetSnapCountsBySeason(playerIds, season);
        public Task<double> GetSnapsByGame(int playerId, int season, int week) => statisticsRepository.GetSnapsByGame(playerId, season, week);
        public Task<List<T>> GetSeasonDataByTeamIdAndPosition<T>(int teamId, Position position, int season) => statisticsRepository.GetSeasonDataByTeamIdAndPosition<T>(teamId, position, season);
        public Task<double> GetYearsExperience(int playerId, Position position) => statisticsRepository.GetYearsExperience(playerId, position);
        public Task<IEnumerable<StarterMissedGames>> GetCurrentStartersThatMissedGamesLastSeason(int currentSeason, int previousSeason, int maxGames, double avgProjection) => statisticsRepository.GetCurrentStartersThatMissedGamesLastSeason(currentSeason, previousSeason, maxGames, avgProjection);
        public async Task<IEnumerable<SeasonADP>> GetAdpByPosition(int season, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.GetAdpByPosition(season);

            return await statisticsRepository.GetAdpByPosition(season, position.ToString());
        }

        public async Task<bool> DeleteAdpByPosition(int season, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.DeleteAdpByPosition(season);

            return await statisticsRepository.DeleteAdpByPosition(season, position.ToString());
        }

        public async Task<IEnumerable<ConsensusProjections>> GetConsensusProjectionsByPosition(int season, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.GetConsensusProjectionsByPosition(season);

            return await statisticsRepository.GetConsensusProjectionsByPosition(season, position.ToString());
        }

        public async Task<bool> DeleteConsensusProjectionsByPosition(int season, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.DeleteConsensusProjectionsByPosition(season);

            return await statisticsRepository.DeleteConsensusProjectionsByPosition(season, position.ToString());
        }

        public async Task<IEnumerable<ConsensusWeeklyProjections>> GetConsensusWeeklyProjectionsByPosition(int season, int week, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.GetConsensusWeeklyProjectionsByPosition(season, week);

            return await statisticsRepository.GetConsensusWeeklyProjectionsByPosition(season, week, position.ToString());
        }

        public async Task<bool> DeleteConsensusWeeklyProjectionsByPosition(int season, int week, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.DeleteConsensusWeeklyProjectionsByPosition(season, week);

            return await statisticsRepository.DeleteConsensusWeeklyProjectionsByPosition(season, week, position.ToString());
        }

        public async Task<List<T>> GetWeeklyData<T>(Position position, int playerId, string team)
        {
            var weeklyData = await GetWeeklyDataByPlayer<T>(position, playerId, _season.CurrentSeason);
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
                    else if (teamChange.NewTeam == team && week >= teamChange.WeekEffective)
                        filteredData.Add(data);
                }
                return filteredData;
            }
            return weeklyData;
        }

        public async Task<double> GetAverageGamesMissed(int playerId)
        {
            var minGames = (await settingsService.GetSeasonTunings(_season.CurrentSeason)).MinGamesForMissedAverage;
            var player = await playersService.GetPlayer(playerId);
            _ = Enum.TryParse(player.Position, out Position position);
            var gamesPerSeason = await statisticsRepository.GetGamesPerSeason(playerId, position, minGames);

            var diff = 0.0;

            foreach (var gs in gamesPerSeason)
            {
                diff += await playersService.GetGamesBySeason(gs.Season) - gs.Games;
            }

            return gamesPerSeason.Any() ? diff / gamesPerSeason.Count() : 0;
        }
    }
}
