using Football.Data.Models;
using Football.Enums;

namespace Football.Fantasy.Interfaces
{
     public interface IStatisticsService
    {
        public Task<List<SeasonDataQB>> GetSeasonDataQBBySeason(int season);
        public Task<List<SeasonDataQB>> GetSeasonDataQB(int playerId);
        public Task<List<SeasonDataRB>> GetSeasonDataRBBySeason(int season);
        public Task<List<SeasonDataRB>> GetSeasonDataRB(int playerId);
        public Task<List<SeasonDataWR>> GetSeasonDataWRBySeason(int season);
        public Task<List<SeasonDataWR>> GetSeasonDataWR(int playerId);
        public Task<List<SeasonDataTE>> GetSeasonDataTEBySeason(int season);
        public Task<List<SeasonDataTE>> GetSeasonDataTE(int playerId);
        public Task<List<SeasonDataDST>> GetSeasonDataDSTBySeason(int season);
        public Task<List<SeasonDataDST>> GetSeasonDataDST(int playerId);
        public Task<List<T>> GetWeeklyData<T>(PositionEnum position, int season, int week);
        public Task<List<T>> GetWeeklyData<T>(PositionEnum position, int playerId);
        public Task<List<GameResult>> GetGameResults(int season, int week);
        public Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week);
       
    }
}
