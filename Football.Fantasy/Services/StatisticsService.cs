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
        public async Task<SeasonDataQB> GetSeasonDataQB(int playerId, int season)
        {
            return await _statisticsRepository.GetSeasonDataQB(playerId, season);
        }
        public async Task<List<SeasonDataQB>> GetSeasonDataQB(int playerId)
        {
            return await _statisticsRepository.GetSeasonDataQB(playerId);
        }
        public async Task<SeasonDataRB> GetSeasonDataRB(int playerId, int season)
        {
            return await _statisticsRepository.GetSeasonDataRB(playerId, season);
        }
        public async Task<List<SeasonDataRB>> GetSeasonDataRB(int playerId)
        {
            return await _statisticsRepository.GetSeasonDataRB(playerId);
        }
        public async Task<SeasonDataWR> GetSeasonDataWR(int playerId, int season)
        {
            return await _statisticsRepository.GetSeasonDataWR(playerId, season);
        }
        public async Task<List<SeasonDataWR>> GetSeasonDataWR(int playerId)
        {
            return await _statisticsRepository.GetSeasonDataWR(playerId);
        }
        public async Task<SeasonDataTE> GetSeasonDataTE(int playerId, int season)
        {
            return await _statisticsRepository.GetSeasonDataTE(playerId, season);
        }
        public async Task<List<SeasonDataTE>> GetSeasonDataTE(int playerId)
        {
            return await _statisticsRepository.GetSeasonDataTE(playerId);
        }
        public async Task<SeasonDataDST> GetSeasonDataDST(int playerId, int season)
        {
            return await _statisticsRepository.GetSeasonDataDST(playerId, season);
        }
        public async Task<List<SeasonDataDST>> GetSeasonDataDST(int playerId)
        {
            return await _statisticsRepository.GetSeasonDataDST(playerId);
        }


    }
}
