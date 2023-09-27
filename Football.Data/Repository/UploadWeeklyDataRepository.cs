using Football.Data.Interfaces;
using Football.Data.Models;
using Dapper;
using System.Data;

namespace Football.Data.Repository
{
    public class UploadWeeklyDataRepository : IUploadWeeklyDataRepository
    {
        private readonly IDbConnection _dbConnection;
        public UploadWeeklyDataRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<int> UploadWeeklyQBData(List<WeeklyDataQB> players, List<int> ignoreList)
        {
            var query = $@"INSERT INTO [dbo].WeeklyQBData (Season, Week, PlayerId, Name, Completions, Attempts, Yards, TD, Int, Sacks, RushingAttempts, RushingYards, RushingTD, Fumbles)
                        VALUES (@Season, @week, @PlayerId, @Name, @Completions, @Attempts, @Yards, @TD, @Int, @Sacks, @RushingAttempts, @RushingYards, @RushingTD, @Fumbles)";
            var count = 0;
            foreach(var player in players)
            {
                count += ignoreList.Contains(player.PlayerId) ? await _dbConnection.ExecuteAsync(query, player) : 0;
            }
            return count;

        }
        public async Task<int> UploadWeeklyRBData(List<WeeklyDataRB> players, List<int> ignoreList)
        {
            var query = $@"INSERT INTO [dbo].WeeklyRBData (Season, Week, PlayerId, Name, RushingAtt, RushingYds, RushingTD, Receptions, Targets, Yards, ReceivingTD, Fumbles)
                            VALUES(@Season, @Week, @PlayerId, @Name, @RushingAtt, @RushingYds, @RushingTD, @Receptions, @Targets, @Yards, @ReceivingTD, @Fumbles)";
            var count = 0;
            foreach (var player in players)
            {
                count += ignoreList.Contains(player.PlayerId) ? await _dbConnection.ExecuteAsync(query, player) : 0;
            }
            return count;
        }
        public async Task<int> UploadWeeklyWRData(List<WeeklyDataWR> players, List<int> ignoreList)
        {
            var query = $@"INSERT INTO [dbo].WeeklyWRData (Season, Week, PlayerId, Name, Receptions, Targets, Yards, Long, TD, RushingAtt, RushingYds, RushingTD, Fumbles)
                        VALUES (@Season, @Week, @PlayerId, @Name, @Receptions, @Targets, @Yards, @Long, @TD, @RushingAtt, @RushingYds, @RushingTD, @Fumbles)";
            var count = 0;
            foreach (var player in players)
            {
                count += ignoreList.Contains(player.PlayerId) ? await _dbConnection.ExecuteAsync(query, player) : 0;
            }
            return count;
        }
        public async Task<int> UploadWeeklyTEData(List<WeeklyDataTE> players, List<int> ignoreList)
        {
            var query = $@"INSERT INTO [dbo].WeeklyTEData (Season, Week, PlayerId, Name, Receptions, Targets, Yards, Long, TD, RushingAtt, RushingYds, RushingTD, Fumbles)
                        VALUES (@Season, @Week, @PlayerId, @Name, @Receptions, @Targets, @Yards, @Long, @TD, @RushingAtt, @RushingYds, @RushingTD, @Fumbles)";
            var count = 0;
            foreach (var player in players)
            {
                count += ignoreList.Contains(player.PlayerId) ? await _dbConnection.ExecuteAsync(query, player) : 0;
            }
            return count;

        }
        public async Task<int> UploadWeeklyDSTData(List<WeeklyDataDST> players)
        {
            var query = $@"INSERT INTO [dbo].WeeklyDSTData (Season, Week, PlayerId, Name, Sacks, Ints, FumblesRecovered, ForcedFumbles, DefensiveTD, Safties, SpecialTD)
                        VALUES(@Season, @Week, @PlayerId, @Name, @Sacks, @Ints, @FumblesRecovered, @ForcedFumbles, @DefensiveTD, @Safties, @SpecialTD)";
            var count = 0;
            foreach (var player in players)
            {
                count += await _dbConnection.ExecuteAsync(query, player);
            }
            return count;
        }

        public async Task<int> UploadWeeklyGameResults(List<GameResult> results)
        {
            var query = $@"INSERT INTO [dbo].GameResults
                        (Season, WinnerId, LoserId, HomeTeamId, AwayTeamId,
                        Week, Day, Date, Time, Winner, Loser, WinnerPoints, LoserPoints,
                        WinnerYards, LoserYards)
                        VALUES
                        (@Season, @WinnerId, @LoserId, @HomeTeamId, @AwayTeamId,
                        @Week, @Day, @Date, @Time, @Winner, @Loser, @WinnerPoints, @LoserPoints,
                        @WinnerYards, @LoserYards)";
            var count = 0;
            foreach (var r in results)
            {
                count += await _dbConnection.ExecuteAsync(query, r);
            }
            return count;
        }
    }
}
