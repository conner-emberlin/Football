using Football.Shared.Models.Fantasy;
using Football.Shared.Models.Teams;
using Football.Client.Interfaces;
using Football.Shared.Models.Players;

namespace Football.Client.Services
{
    public class TeamsService(IRequests requests) : ITeamsService
    {
        public async Task<List<TeamMapModel>?> GetAllTeamsRequest() => await requests.Get<List<TeamMapModel>?>("/team/all");
        public async Task<List<GameResultModel>?> GetGameResultsRequest() => await requests.Get<List<GameResultModel>?>("/team/game-results");
        public async Task<List<TeamRecordModel>?> GetTeamRecordsRequest() => await requests.Get<List<TeamRecordModel>?>("/team/team-records");
        public async Task<List<FantasyPerformanceModel>?> GetFantasyPerformancesRequest(int teamId) => await requests.Get<List<FantasyPerformanceModel>?>("/team/fantasy-performances/" + teamId.ToString());
        public async Task<List<ScheduleDetailsModel>?> GetScheduleDetailsRequest() => await requests.Get<List<ScheduleDetailsModel>?>("/team/schedule-details/current");
        public async Task<List<WeeklyFantasyModel>?> GetTeamWeeklyFantasyRequest(string team, string week) => await requests.Get<List<WeeklyFantasyModel>?>(string.Format("{0}/{1}/{2}", "/team/weekly-fantasy", team, week));
        public async Task<TeamLeagueInformationModel?> GetTeamLeagueInformationRequest(int teamId) => await requests.Get<TeamLeagueInformationModel?>("/team/league-information/" + teamId.ToString());
        public async Task<TeamRecordModel?> GetTeamRecordInDivisionRequest(int teamId) => await requests.Get<TeamRecordModel?>("/team/team-records/division/" + teamId.ToString());
        public async Task<List<TeamDepthChartModel>?> GetTeamDepthChartRequest(string teamId) => await requests.Get<List<TeamDepthChartModel>?>("/team/depth-chart/" + teamId);
        public async Task<List<MatchupRankingModel>?> GetROSMatchupRankingsRequest(string teamId) => await requests.Get<List<MatchupRankingModel>?>("/team/ros-matchup-rankings/" + teamId);
        public async Task<List<ScheduleModel>?> GetUpcomingGamesRequest(int teamPlayerId) => await requests.Get<List<ScheduleModel>?>("/team/upcoming-games/" + teamPlayerId.ToString());

    }
}
