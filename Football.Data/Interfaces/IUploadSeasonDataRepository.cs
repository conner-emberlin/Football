using Football.Players.Models;

namespace Football.Data.Interfaces
{
    public interface IUploadSeasonDataRepository
    {
        Task<int> UploadSeasonQBData(IEnumerable<SeasonDataQB> players);
        Task<int> UploadSeasonRBData(IEnumerable<SeasonDataRB> players);
        Task<int> UploadSeasonWRData(IEnumerable<SeasonDataWR> players);
        Task<int> UploadSeasonTEData(IEnumerable<SeasonDataTE> players);
        Task<int> UploadSeasonDSTData(List<SeasonDataDST> players);
        Task<int> UploadCurrentTeams(List<PlayerTeam> teams);
        Task<int> UploadSchedule(List<Schedule> schedules);
        Task<int> UploadScheduleDetails(List<ScheduleDetails> scheduleDetails);
        Task<int> UploadADP(List<SeasonADP> adp);
        Task<int> UploadConsensusProjections(List<ConsensusProjections> projections);
    }
}
