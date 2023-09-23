using Football.Data.Models;
using Football.Fantasy.Interfaces;

namespace Football.Fantasy.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        public StatisticsService(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
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
        public async Task<List<WeeklyDataQB>> GetWeeklyDataQB(int season, int week) => await _statisticsRepository.GetWeeklyDataQB(season, week);
        public async Task<List<WeeklyDataRB>> GetWeeklyDataRB(int season, int week) => await _statisticsRepository.GetWeeklyDataRB(season, week);
        public async Task<List<WeeklyDataWR>> GetWeeklyDataWR(int season, int week) => await _statisticsRepository.GetWeeklyDataWR(season, week);
        public async Task<List<WeeklyDataTE>> GetWeeklyDataTE(int season, int week) => await _statisticsRepository.GetWeeklyDataTE(season, week);
        public async Task<List<WeeklyDataDST>> GetWeeklyDataDST(int season, int week) => await _statisticsRepository.GetWeeklyDataDST(season, week);
        public async Task<List<WeeklyDataQB>> GetWeeklyDataQB(int playerId) => await _statisticsRepository.GetWeeklyDataQB(playerId);
        public async Task<List<WeeklyDataRB>> GetWeeklyDataRB(int playerId) => await _statisticsRepository.GetWeeklyDataRB(playerId);
        public async Task<List<WeeklyDataWR>> GetWeeklyDataWR(int playerId) => await _statisticsRepository.GetWeeklyDataWR(playerId);
        public async Task<List<WeeklyDataTE>> GetWeeklyDataTE(int playerId) => await _statisticsRepository.GetWeeklyDataTE(playerId);
        public async Task<List<WeeklyDataDST>> GetWeeklyDataDST(int playerId) => await _statisticsRepository.GetWeeklyDataDST(playerId);
        public async Task<List<GameResult>> GetGameResults(int season, int week) => await _statisticsRepository.GetGameResults(season, week);

    }
}
