using Football.Shared.Models.Operations;

namespace Football.Client.Interfaces
{
    public interface IOperationsService
    {
        Task<int> PutSeasonAdpRequest(string position);
        Task<TuningsModel?> GetSeasonTuningsRequest();
        Task<bool> PostSeasonTuningsRequest(TuningsModel model);
        Task<SeasonAdjustmentsModel?> GetSeasonAdjustmentsRequest();
        Task<bool> PostSeasonAdjustmentsRequest(SeasonAdjustmentsModel model);
        Task<WeeklyTuningsModel?> GetWeeklyTuningsRequest();
        Task<bool> PostWeeklyTuningsRequest(WeeklyTuningsModel model);
        Task<int> PutSeasonConsensusProjectionsRequest(string position);
        Task<int> PutWeeklyConsensusProjectionsRequest(string position);
    }
}
