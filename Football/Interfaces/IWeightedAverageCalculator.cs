using Football.Models;

namespace Football.Interfaces
{
    public interface IWeightedAverageCalculator
    {
        public PassingStatistic WeightedAverage(List<PassingStatisticWithSeason> passing);
        public RushingStatistic WeightedAverage(List<RushingStatisticWithSeason> rushing);
        public ReceivingStatistic WeightedAverage(List<ReceivingStatisticWithSeason> receving);
        public FantasyPoints WeightedAverage(Player player);
        
    }
}
