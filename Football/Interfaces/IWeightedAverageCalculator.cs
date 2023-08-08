using Football.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Interfaces
{
    public interface IWeightedAverageCalculator
    {
        public Task<PassingStatistic> PassingWeightedAverage(int playerId);
        public Task<RushingStatistic> RushingWeightedAverage(int playerId);
        public Task<ReceivingStatistic> ReceivingWeightedAverage(int playerId);
       // public Task<FantasyPoints> FantasyWeightedAversage(int playerId, string position);
    }
}
