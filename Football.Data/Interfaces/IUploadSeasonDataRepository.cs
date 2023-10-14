using Football.Players.Models;
using Football.Data.Models;

namespace Football.Data.Interfaces
{
    public interface IUploadSeasonDataRepository
    {
        public Task<int> UploadSeasonQBData(List<SeasonDataQB> players, List<int> ignoreList);
        public Task<int> UploadSeasonRBData(List<SeasonDataRB> players, List<int> ignoreList);
        public Task<int> UploadSeasonWRData(List<SeasonDataWR> players, List<int> ignoreList);
        public Task<int> UploadSeasonTEData(List<SeasonDataTE> players, List<int> ignoreList);
        public Task<int> UploadSeasonDSTData(List<SeasonDataDST> players);
        public Task<int> UploadCurrentTeams(List<PlayerTeam> teams);
        public Task<int> UploadSchedule(List<Schedule> schedules);
        public Task<int> UploadScheduleDetails(List<ScheduleDetails> scheduleDetails);
        public Task<int> UploadADP(List<SeasonADP> adp);
    }
}
