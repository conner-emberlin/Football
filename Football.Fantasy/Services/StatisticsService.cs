using Football.Data.Models;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;

namespace Football.Fantasy.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IPlayersService _playersService;
        public StatisticsService(IStatisticsRepository statisticsRepository, IPlayersService playersService)
        {
            _statisticsRepository = statisticsRepository;
            _playersService = playersService;
        }
        public async Task<List<SeasonDataQB>> GetSeasonDataQBBySeason(int season) => await _statisticsRepository.GetSeasonDataQBBySeason(season);
        public async Task<List<SeasonDataQB>> GetSeasonDataQB(int playerId) => await _statisticsRepository.GetSeasonDataQB(playerId);
        public async Task<List<SeasonDataRB>> GetSeasonDataRBBySeason(int season) => await _statisticsRepository.GetSeasonDataRBBySeason(season);
        public async Task<List<SeasonDataRB>> GetSeasonDataRB(int playerId) => await _statisticsRepository.GetSeasonDataRB(playerId);
        public async Task<List<SeasonDataWR>> GetSeasonDataWRBySeason(int season) => await _statisticsRepository.GetSeasonDataWRBySeason(season);
        public async Task<List<SeasonDataWR>> GetSeasonDataWR(int playerId) => await _statisticsRepository.GetSeasonDataWR(playerId);
        public async Task<List<SeasonDataTE>> GetSeasonDataTEBySeason(int season) => await _statisticsRepository.GetSeasonDataTEBySeason(season);
        public async Task<List<SeasonDataTE>> GetSeasonDataTE(int playerId) => await _statisticsRepository.GetSeasonDataTE(playerId);
        public async Task<List<SeasonDataDST>> GetSeasonDataDSTBySeason( int season) => await _statisticsRepository.GetSeasonDataDSTBySeason(season);
        public async Task<List<SeasonDataDST>> GetSeasonDataDST(int playerId) => await _statisticsRepository.GetSeasonDataDST(playerId);
        public async Task<List<T>> GetWeeklyData<T>(PositionEnum position, int playerId) => await _statisticsRepository.GetWeeklyData<T>(position, playerId);
        public async Task<List<T>> GetWeeklyData<T>(PositionEnum position, int season, int week) => await _statisticsRepository.GetWeeklyData<T>(position, season, week);
        public async Task<List<GameResult>> GetGameResults(int season, int week) => await _statisticsRepository.GetGameResults(season, week);
        public async Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week) => await _statisticsRepository.GetWeeklyRosterPercentages(season, week);

    }
}
