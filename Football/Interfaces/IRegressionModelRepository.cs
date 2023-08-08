﻿using Football.Models;
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
        public Task<PassingStatistic> GetPassingStatistic(int playerId, int season);

        public Task<PassingStatisticWithSeason> GetPassingStatisticWithSeason(int playerId, int season);
        public Task<RushingStatistic> GetRushingStatistic(int playerId, int season);

        public Task<RushingStatisticWithSeason> GetRushingStatisticWithSeason(int playerId, int season);
        public Task<ReceivingStatistic> GetReceivingStatistic(int playerId, int season);
        public Task<ReceivingStatisticWithSeason> GetReceivingStatisticWithSeason(int playerId, int season);

    }
}
