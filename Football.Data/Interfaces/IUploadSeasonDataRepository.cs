using Football.Players.Models;
using Football.Projections.Models;

namespace Football.Data.Interfaces
{
    public interface IUploadSeasonDataRepository
    {
        public Task<int> UploadSeasonQBData(IEnumerable<SeasonDataQB> players);
        public Task<int> UploadSeasonRBData(IEnumerable<SeasonDataRB> players);
        public Task<int> UploadSeasonWRData(IEnumerable<SeasonDataWR> players);
        public Task<int> UploadSeasonTEData(IEnumerable<SeasonDataTE> players);
        public Task<int> UploadSeasonDSTData(List<SeasonDataDST> players);
        public Task<int> UploadCurrentTeams(List<PlayerTeam> teams);
        public Task<int> UploadSchedule(List<Schedule> schedules);
        public Task<int> UploadScheduleDetails(List<ScheduleDetails> scheduleDetails);
        public Task<int> UploadADP(List<SeasonADP> adp);
        public Task<int> UploadConsensusProjections(List<ConsensusProjections> projections);
    }
}
