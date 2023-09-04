using Football.Data.Models;

namespace Football.Fantasy.Interfaces
{
     public interface IStatisticsService
    {
        public Task<SeasonDataQB> GetSeasonDataQB(int playerId, int season);
        public Task<List<SeasonDataQB>> GetSeasonDataQB(int playerId);
        public Task<SeasonDataRB> GetSeasonDataRB(int playerId, int season);
        public Task<List<SeasonDataRB>> GetSeasonDataRB(int playerId);
        public Task<SeasonDataWR> GetSeasonDataWR(int playerId, int season);
        public Task<List<SeasonDataWR>> GetSeasonDataWR(int playerId);
        public Task<SeasonDataTE> GetSeasonDataTE(int playerId, int season);
        public Task<List<SeasonDataTE>> GetSeasonDataTE(int playerId);
        public Task<SeasonDataDST> GetSeasonDataDST(int playerId, int season);
        public Task<List<SeasonDataDST>> GetSeasonDataDST(int playerId);
    }
}
