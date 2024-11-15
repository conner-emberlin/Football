using Football.Enums;
using Football.Players.Models;

namespace Football.Data.Interfaces
{
    public interface IUploadWeeklyDataService
    {
        Task<int> UploadWeeklyQBData(int season, int week, List<int> ignoreList);
        Task<int> UploadWeeklyRBData(int season, int week, List<int> ignoreList);
        Task<int> UploadWeeklyWRData(int season, int week, List<int> ignoreList);
        Task<int> UploadWeeklyTEData(int season, int week, List<int> ignoreList);
        Task<int> UploadWeeklyDSTData(int season, int week);
        Task<int> UploadWeeklyKData(int season, int week);
        Task<int> UploadWeeklyGameResults(int season, int week);
        Task<int> UploadWeeklyRosterPercentages(int season, int week, string position);
        Task<int> UploadWeeklySnapCounts(int season, int week, string position);
        Task<int> UploadWeeklyRedZoneRB(int season, int week, int yardline);
        Task<int> UploadConsensusWeeklyProjections(int week, string position, List<int> ignoreList);
        Task<int> UploadPlayerTeams(int season, int week, Position position);
    }
}
