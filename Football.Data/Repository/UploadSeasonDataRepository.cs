﻿using Football.Data.Interfaces;
using Football.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Football.Data.Repository
{
    public class UploadSeasonDataRepository : IUploadSeasonDataRepository
    {
        private readonly IDbConnection _dbConnection;
        public UploadSeasonDataRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<int> UploadSeasonQBData(List<SeasonDataQB> players)
        {
            var query = $@"INSERT INTO [dbo].SeasonQBData (Season, PlayerId, Name, Completions, Attempts, Yards, TD, Int, Sacks, RushingAttempts, RushingYards, RushingTD, Fumbles, Games)
                        VALUES (@Season, @PlayerId, @Name, @Completions, @Attempts, @Yards, @TD, @Int, @Sacks, @RushingAttempts, @RushingYards, @RushingTD, @Fumbles, @Games)";
            var count = 0;
            foreach (var player in players)
            {
                count += await _dbConnection.ExecuteAsync(query, player);
            }
            return count;

        }
        public async Task<int> UploadSeasonRBData(List<SeasonDataRB> players)
        {
            var query = $@"INSERT INTO [dbo].SeasonRBData (Season, PlayerId, Name, RushingAtt, RushingYds, RushingTD, Receptions, Targets, Yards, ReceivingTD, Fumbles, Games)
                            VALUES(@Season, @PlayerId, @Name, @RushingAtt, @RushingYds, @RushingTD, @Receptions, @Targets, @Yards, @ReceivingTD, @Fumbles, @Games)";
            var count = 0;
            foreach (var player in players)
            {
                count += await _dbConnection.ExecuteAsync(query, player);
            }
            return count;
        }
        public async Task<int> UploadSeasonWRData(List<SeasonDataWR> players)
        {
            var query = $@"INSERT INTO [dbo].SeasonWRData (Season, PlayerId, Name, Receptions, Targets, Yards, Long, TD, RushingAtt, RushingYds, RushingTD, Fumbles, Games)
                        VALUES (@Season, @PlayerId, @Name, @Receptions, @Targets, @Yards, @Long, @TD, @RushingAtt, @RushingYds, @RushingTD, @Fumbles, @Games)";
            var count = 0;
            foreach (var player in players)
            {
                count += await _dbConnection.ExecuteAsync(query, player);
            }
            return count;
        }
        public async Task<int> UploadSeasonTEData(List<SeasonDataTE> players)
        {
            var query = $@"INSERT INTO [dbo].SeasonTEData (Season, PlayerId, Name, Receptions, Targets, Yards, Long, TD, RushingAtt, RushingYds, RushingTD, Fumbles, Games)
                        VALUES (@Season, @PlayerId, @Name, @Receptions, @Targets, @Yards, @Long, @TD, @RushingAtt, @RushingYds, @RushingTD, @Fumbles, @Games)";
            var count = 0;
            foreach (var player in players)
            {
                count += await _dbConnection.ExecuteAsync(query, player);
            }
            return count;

        }
        public async Task<int> UploadSeasonDSTData(List<SeasonDataDST> players)
        {
            var query = $@"INSERT INTO [dbo].SeasonDSTData (Season, PlayerId, Name, Sacks, Ints, FumblesRecovered, ForcedFumbles, DefensiveTD, Safties, SpecialTD, Games)
                        VALUES(@Season, @PlayerId, @Name, @Sacks, @Ints, @FumblesRecovered, @ForcedFumbles, @DefensiveTD, @Safties, @SpecialTD, @Games)";
            var count = 0;
            foreach (var player in players)
            {
                count += await _dbConnection.ExecuteAsync(query, player);
            }
            return count;
        }








    }
}