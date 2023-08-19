using Football.Models;

namespace Football.Interfaces
{
    public interface IWeightedAverageCalculator
    {
        public PassingStatistic PassingWeightedAverage(List<PassingStatisticWithSeason> passing);
        public RushingStatistic RushingWeightedAverage(List<RushingStatisticWithSeason> rushing);
        public ReceivingStatistic ReceivingWeightedAverage(List<ReceivingStatisticWithSeason> receving);
        public FantasyPoints FantasyWeightedAverage(Player player);
    }
}
