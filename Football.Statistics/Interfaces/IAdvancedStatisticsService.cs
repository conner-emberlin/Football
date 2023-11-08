using Football.Statistics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Statistics.Interfaces
{
    public interface IAdvancedStatisticsService
    {
        public Task<List<QBValue>> FiveThirtyEightQBValue();
        public Task<double> FiveThirtyEightQBValue(int playerId);
        public Task<List<QBValue>> PasserRating();
        public Task<double> PasserRating(int playerId);
    }
}
