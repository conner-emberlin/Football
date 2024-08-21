namespace Football.Data.Interfaces
{
    public interface IUploadWeeklyDataService
    {
        public Task<int> UploadWeeklyQBData(int season, int week, List<int> ignoreList);
        public Task<int> UploadWeeklyRBData(int season, int week, List<int> ignoreList);
        public Task<int> UploadWeeklyWRData(int season, int week, List<int> ignoreList);
        public Task<int> UploadWeeklyTEData(int season, int week, List<int> ignoreList);
        public Task<int> UploadWeeklyDSTData(int season, int week);
        public Task<int> UploadWeeklyKData(int season, int week);
        public Task<int> UploadWeeklyGameResults(int season, int week);
        public Task<int> UploadWeeklyRosterPercentages(int season, int week, string position);
        public Task<int> UploadWeeklySnapCounts(int season, int week, string position);
        public Task<int> UploadWeeklyRedZoneRB(int season, int week, int yardline);
        public Task<int> UploadConsensusWeeklyProjections(int week, string position);
    }
}
