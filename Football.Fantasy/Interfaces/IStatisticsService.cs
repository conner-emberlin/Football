using Football.Data.Models;

namespace Football.Fantasy.Interfaces
{
     public interface IStatisticsService
    {
        public Task<SeasonDataQB> GetSeasonDataQB(int playerId, int season);
        public Task<List<SeasonDataQB>> GetSeasonDataQB(int season);
        public Task<SeasonDataRB> GetSeasonDataRB(int playerId, int season);
        public Task<List<SeasonDataRB>> GetSeasonDataRB(int season);
        public Task<SeasonDataWR> GetSeasonDataWR(int playerId, int season);
        public Task<List<SeasonDataWR>> GetSeasonDataWR(int season);
        public Task<SeasonDataTE> GetSeasonDataTE(int playerId, int season);
        public Task<List<SeasonDataTE>> GetSeasonDataTE(int season);
        public Task<SeasonDataDST> GetSeasonDataDST(int playerId, int season);
        public Task<List<SeasonDataDST>> GetSeasonDataDST(int season);
    }
}
