using Football.Api.Models;
using Football.Enums;
using Football.Fantasy.Models;
using Football.Projections.Models;

namespace Football.UI
{
    public interface IRequests
    {
        public Task<List<SnapCountAnalysisModel>?> SnapCountAnalysisRequest(string position);
        public Task<List<FantasyAnalysisModel>?> GetFantasyAnalysesRequest(Position position);
        public Task<List<SeasonFantasy>?> GetSeasonTotals(string season = "");
        public Task<List<WeeklyFantasy>?> GetWeeklyFantasy(string week, string season = "");
        public Task<List<WeeklyProjectionAnalysis>?> GetCurrentSeasonWeeklyAnalysisRequest(string position);
        public Task<List<SeasonProjectionAnalysis>?> GetPreviousSeasonsAnalysisRequest(string position);
        public Task<List<TargetShareModel>?> GetTargetShareRequest();
    }
}
