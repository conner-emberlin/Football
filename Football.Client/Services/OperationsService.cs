using Football.Shared.Models.Operations;
using Football.Client.Interfaces;

namespace Football.Client.Services
{
    public class OperationsService(IRequests requests) : IOperationsService
    {
        public async Task<TuningsModel?> GetSeasonTuningsRequest() => await requests.Get<TuningsModel>("/operations/season-tunings");
        public async Task<bool> PostSeasonTuningsRequest(TuningsModel tunings) => await requests.Post<bool, TuningsModel>("/operations/season-tunings", tunings);
        public async Task<WeeklyTuningsModel?> GetWeeklyTuningsRequest() => await requests.Get<WeeklyTuningsModel>("/operations/weekly-tunings");
        public async Task<bool> PostWeeklyTuningsRequest(WeeklyTuningsModel tunings) => await requests.Post<bool, WeeklyTuningsModel>("/operations/weekly-tunings", tunings);
        public async Task<int> PutSeasonAdpRequest(string position) => await requests.Put<int>("/operations/refresh-adp/" + position);
        public async Task<int> PutSeasonConsensusProjectionsRequest(string position) => await requests.Put<int>("/operations/refresh-consensus-projections/" + position);
    }
}
