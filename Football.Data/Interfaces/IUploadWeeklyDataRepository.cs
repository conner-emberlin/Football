using Football.Data.Models;

namespace Football.Data.Interfaces
{
    public interface IUploadWeeklyDataRepository
    {
        public Task<int> UploadWeeklyQBData(List<WeeklyDataQB> players);
        public Task<int> UploadWeeklyRBData(List<WeeklyDataRB> players);
        public Task<int> UploadWeeklyWRData(List<WeeklyDataWR> players);
        public Task<int> UploadWeeklyTEData(List<WeeklyDataTE> players);
        public Task<int> UploadWeeklyDSTData(List<WeeklyDataDST> players);
        public Task<int> UploadWeeklyGameResults(List<GameResult> results);
    }
}
