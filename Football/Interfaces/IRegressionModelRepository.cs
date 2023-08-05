using Football.Models;
using Football.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Interfaces
{
    public interface IRegressionModelRepository
    {
        public PassingStatistic GetPassingStatistic(int playerId, int season);
        public RushingStatistic GetRushingStatistic(int playerId, int season);
        public ReceivingStatistic GetReceivingStatistic(int playerId, int season);
    }
}
