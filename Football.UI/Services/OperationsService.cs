using Football.Api.Models.Operations;
using Football.UI.Interfaces;

namespace Football.UI.Services
{
    public class OperationsService(IRequests requests) : IOperationsService
    {
        public async Task<TuningsModel?> GetSeasonTuningsRequest() => await requests.Get<TuningsModel>("/operations/season-tunings");
        public async Task<bool> PostSeasonTuningsRequest(TuningsModel tunings) => await requests.Post<bool, TuningsModel>("/operations/season-tunings", tunings);
        public async Task<WeeklyTuningsModel?> GetWeeklyTuningsRequest() => await requests.Get<WeeklyTuningsModel>("/operations/weekly-tunings");
        public async Task<bool> PostWeeklyTuningsRequest(WeeklyTuningsModel tunings) => await requests.Post<bool, WeeklyTuningsModel>("/operations/weekly-tunings", tunings);
        public async Task<int> PutSeasonAdpRequest(string position) => await requests.Put<int>("/operations/refresh-adp/" + position);
    }
}
