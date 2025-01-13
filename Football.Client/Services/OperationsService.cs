using Football.Shared.Models.Operations;
using Football.Client.Interfaces;

namespace Football.Client.Services
{
    public class OperationsService(IRequests requests) : IOperationsService
    {
        public async Task<TuningsModel?> GetSeasonTuningsRequest() => await requests.Get<TuningsModel>("/operations/season-tunings");
        public async Task<bool> PostSeasonTuningsRequest(TuningsModel tunings) => await requests.Post<bool, TuningsModel>("/operations/season-tunings", tunings);
        public async Task<SeasonAdjustmentsModel?> GetSeasonAdjustmentsRequest() => await requests.Get<SeasonAdjustmentsModel>("/operations/season-adjustments");
        public async Task<bool> PostSeasonAdjustmentsRequest(SeasonAdjustmentsModel seasonAdjustments) => await requests.Post<bool, SeasonAdjustmentsModel>("/operations/season-adjustments", seasonAdjustments);
        public async Task<WeeklyTuningsModel?> GetWeeklyTuningsRequest() => await requests.Get<WeeklyTuningsModel>("/operations/weekly-tunings");
        public async Task<bool> PostWeeklyTuningsRequest(WeeklyTuningsModel tunings) => await requests.Post<bool, WeeklyTuningsModel>("/operations/weekly-tunings", tunings);
        public async Task<int> PutSeasonAdpRequest(string position) => await requests.Put<int>("/operations/refresh-adp/" + position);
        public async Task<int> PutSeasonConsensusProjectionsRequest(string position) => await requests.Put<int>("/operations/refresh-consensus-projections/" + position);
        public async Task<int> PutWeeklyConsensusProjectionsRequest(string position) => await requests.Put<int>("/operations/refresh-consensus-weekly-projections/" + position);
        public async Task<bool> PostSeasonInfoRequest(SeasonInfoModel seasonInfo) => await requests.Post<bool, SeasonInfoModel>("/operations/season-info", seasonInfo);
        public async Task<SeasonInfoModel?> GetSeasonInfoRequest() => await requests.Get<SeasonInfoModel>("/operations/season-info");
    }
}
