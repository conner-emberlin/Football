using Football.Shared.Models.Operations;

namespace Football.Client.Interfaces
{
    public interface IOperationsService
    {
        public Task<int> PutSeasonAdpRequest(string position);
        public Task<TuningsModel?> GetSeasonTuningsRequest();
        public Task<bool> PostSeasonTuningsRequest(TuningsModel model);
        public Task<WeeklyTuningsModel?> GetWeeklyTuningsRequest();
        public Task<bool> PostWeeklyTuningsRequest(WeeklyTuningsModel model);
        public Task<int> PutSeasonConsensusProjectionsRequest(string position);
    }
}
