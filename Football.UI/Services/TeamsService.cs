using Football.Api.Models.Fantasy;
using Football.Api.Models.Teams;
using Football.UI.Interfaces;

namespace Football.UI.Services
{
    public class TeamsService(IRequests requests) : ITeamsService
    {
        public async Task<List<TeamMapModel>?> GetAllTeamsRequest() => await requests.Get<List<TeamMapModel>?>("/team/all");
        public async Task<List<GameResultModel>?> GetGameResultsRequest() => await requests.Get<List<GameResultModel>?>("/team/game-results");
        public async Task<List<TeamRecordModel>?> GetTeamRecordsRequest() => await requests.Get<List<TeamRecordModel>?>("/team/team-records");
        public async Task<List<FantasyPerformanceModel>?> GetFantasyPerformancesRequest(int teamId) => await requests.Get<List<FantasyPerformanceModel>?>("/team/fantasy-performances/" + teamId.ToString());
        public async Task<List<ScheduleDetailsModel>?> GetScheduleDetailsRequest() => await requests.Get<List<ScheduleDetailsModel>?>("/team/schedule-details/current");

    }
}
