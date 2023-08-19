using Football.Models;

namespace Football.Interfaces
{
    public interface IWeightedAverageCalculator
    {
        public Task<PassingStatistic> PassingWeightedAverage(int playerId);
        public Task<RushingStatistic> RushingWeightedAverage(int playerId);
        public Task<ReceivingStatistic> ReceivingWeightedAverage(int playerId);
        public Task<FantasyPoints> FantasyWeightedAverage(int playerId, string position);
    }
}
