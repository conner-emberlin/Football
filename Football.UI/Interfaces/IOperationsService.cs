using Football.Api.Models.Operations;

namespace Football.UI.Interfaces
{
    public interface IOperationsService
    {
        public Task<int> PutSeasonAdpRequest(string position);
        public Task<TuningsModel?> GetSeasonTuningsRequest();
        public Task<bool> PostSeasonTuningsRequest(TuningsModel model);
        public Task<WeeklyTuningsModel?> GetWeeklyTuningsRequest();
        public Task<bool> PostWeeklyTuningsRequest(WeeklyTuningsModel model);
    }
}
