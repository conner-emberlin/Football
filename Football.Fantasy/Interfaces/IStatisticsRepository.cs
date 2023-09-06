using Football.Data.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IStatisticsRepository
    {
        public Task<List<SeasonDataQB>> GetSeasonDataQBBySeason(int season);
        public Task<List<SeasonDataQB>> GetSeasonDataQB(int playerId);
        public Task<List<SeasonDataWR>> GetSeasonDataWRBySeason(int season);
        public Task<List<SeasonDataWR>> GetSeasonDataWR(int playerId);
        public Task<List<SeasonDataRB>> GetSeasonDataRBBySeason(int season);
        public Task<List<SeasonDataRB>> GetSeasonDataRB(int playerId);
        public Task<List<SeasonDataTE>> GetSeasonDataTEBySeason(int season);
        public Task<List<SeasonDataTE>> GetSeasonDataTE(int playerId);
        public Task<List<SeasonDataDST>> GetSeasonDataDSTBySeason(int season);
        public Task<List<SeasonDataDST>> GetSeasonDataDST(int playerId);
    }
}
