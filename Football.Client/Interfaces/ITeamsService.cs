using Football.Shared.Models.Fantasy;
using Football.Shared.Models.Players;
using Football.Shared.Models.Teams;

namespace Football.Client.Interfaces
{
    public interface ITeamsService
    {
        Task<List<FantasyPerformanceModel>?> GetFantasyPerformancesRequest(int teamId);
        Task<List<TeamMapModel>?> GetAllTeamsRequest();
        Task<List<GameResultModel>?> GetGameResultsRequest();
        Task<List<TeamRecordModel>?> GetTeamRecordsRequest();
        Task<List<ScheduleDetailsModel>?> GetScheduleDetailsRequest();
        Task<List<WeeklyFantasyModel>?> GetTeamWeeklyFantasyRequest(string team, string week);
        Task<TeamLeagueInformationModel?> GetTeamLeagueInformationRequest(int teamId);
        Task<TeamRecordModel?> GetTeamRecordInDivisionRequest(int teamId);
        Task<List<TeamDepthChartModel>?> GetTeamDepthChartRequest(string teamId);
        Task<List<MatchupRankingModel>?> GetROSMatchupRankingsRequest(string teamId);
        Task<List<ScheduleModel>?> GetUpcomingGamesRequest(int teamPlayerId);
    }
}
