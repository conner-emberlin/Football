using Football.Models;

namespace Football.Interfaces
{
    public interface IWeightedAverageCalculator
    {
        public PassingStatistic PassingWeightedAverage(Player player);
        public RushingStatistic RushingWeightedAverage(Player player);
        public ReceivingStatistic ReceivingWeightedAverage(Player player);
        public FantasyPoints FantasyWeightedAverage(Player player);
    }
}
