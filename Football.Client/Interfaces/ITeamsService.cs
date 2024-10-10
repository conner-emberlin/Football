using Football.Shared.Models.Fantasy;
using Football.Shared.Models.Teams;

namespace Football.Client.Interfaces
{
    public interface ITeamsService
    {
        public Task<List<FantasyPerformanceModel>?> GetFantasyPerformancesRequest(int teamId);
        public Task<List<TeamMapModel>?> GetAllTeamsRequest();
        public Task<List<GameResultModel>?> GetGameResultsRequest();
        public Task<List<TeamRecordModel>?> GetTeamRecordsRequest();
        public Task<List<ScheduleDetailsModel>?> GetScheduleDetailsRequest();
        public Task<List<WeeklyFantasyModel>?> GetTeamWeeklyFantasyRequest(string team, string week);
        public Task<TeamLeagueInformationModel?> GetTeamLeagueInformationRequest(int teamId);
        public Task<TeamRecordModel?> GetTeamRecordInDivisionRequest(int teamId);
    }
}
