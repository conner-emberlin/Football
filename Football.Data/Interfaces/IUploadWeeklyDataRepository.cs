using Football.Data.Models;

namespace Football.Data.Interfaces
{
    public interface IUploadWeeklyDataRepository
    {
        public Task<int> UploadWeeklyQBData(List<WeeklyDataQB> players, List<int> ignoreList);
        public Task<int> UploadWeeklyRBData(List<WeeklyDataRB> players, List<int> ignoreList);
        public Task<int> UploadWeeklyWRData(List<WeeklyDataWR> players, List<int> ignoreList);
        public Task<int> UploadWeeklyTEData(List<WeeklyDataTE> players, List<int> ignoreList);
        public Task<int> UploadWeeklyDSTData(List<WeeklyDataDST> players);
        public Task<int> UploadWeeklyGameResults(List<GameResult> results);
        public Task<int> UploadWeeklyRosterPercentages(List<WeeklyRosterPercent> rosterPercentages);
    }
}
