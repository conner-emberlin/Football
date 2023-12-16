using Football.Data.Interfaces;
using Football.Data.Models;
using Dapper;
using System.Data;
using Football.Players.Models;

namespace Football.Data.Repository
{
    public class UploadWeeklyDataRepository(IDbConnection dbConnection) : IUploadWeeklyDataRepository
    {
        public async Task<int> UploadWeeklyQBData(List<WeeklyDataQB> players, List<int> ignoreList)
        {
            var query = $@"INSERT INTO [dbo].WeeklyQBData (Season, Week, PlayerId, Name, Completions, Attempts, Yards, TD, Int, Sacks, RushingAttempts, RushingYards, RushingTD, Fumbles)
                        VALUES (@Season, @week, @PlayerId, @Name, @Completions, @Attempts, @Yards, @TD, @Int, @Sacks, @RushingAttempts, @RushingYards, @RushingTD, @Fumbles)";
            var count = 0;
            foreach(var player in players)
            {
                count += !ignoreList.Contains(player.PlayerId) ? await dbConnection.ExecuteAsync(query, player) : 0;
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
                count += !ignoreList.Contains(player.PlayerId) ? await dbConnection.ExecuteAsync(query, player) : 0;
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
                count += !ignoreList.Contains(player.PlayerId) ? await dbConnection.ExecuteAsync(query, player) : 0;
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
                count += !ignoreList.Contains(player.PlayerId) ? await dbConnection.ExecuteAsync(query, player) : 0;
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
                count += await dbConnection.ExecuteAsync(query, player);
            }
            return count;
        }

        public async Task<int> UploadWeeklyKData(List<WeeklyDataK> players)
        {
            var query = $@"INSERT INTO [dbo].WeeklyKData (Season, Week, PlayerId, Name, FieldGoals, FieldGoalAttempts, OneNineteen, TwentyTwentyNine,
                           ThirtyThirtyNine, FourtyFourtyNine, Fifty, ExtraPoints, ExtraPointAttempts)
                            Values (@Season, @Week, @PlayerId, @Name, @FieldGoals, @FieldGoalAttempts, @OneNineteen, @TwentyTwentyNine,
                           @ThirtyThirtyNine, @FourtyFourtyNine, @Fifty, @ExtraPoints, @ExtraPointAttempts)";
            return await dbConnection.ExecuteAsync(query, players);
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
            return await dbConnection.ExecuteAsync(query, results);
        }

        public async Task<int> UploadWeeklyRosterPercentages(List<WeeklyRosterPercent> rosterPercentages, List<int> ignoreList)
        {
            var query = $@"INSERT INTO [dbo].WeeklyRosterPercentages (Season, Week, PlayerId, Name, RosterPercent)
                            VALUES (@Season, @Week, @PlayerId, @Name, @RosterPercent)";
            var count = 0;
            foreach (var rp in rosterPercentages)
            {
                count += !ignoreList.Contains(rp.PlayerId) ? await dbConnection.ExecuteAsync(query, rp) : 0;
            }
            return count;
        }

        public async Task<int> UploadWeeklySnapCounts(List<SnapCount> snapCounts)
        {
            var query = $@"INSERT INTO [dbo].SnapCount (Season, Week, PlayerId, Name, Position, Snaps)
                            VALUES (@Season, @Week, @PlayerId, @Name, @Position, @Snaps)";
            return await dbConnection.ExecuteAsync(query, snapCounts);
        }

        public async Task<int> UploadWeeklyRedZoneRB(List<WeeklyRedZoneRB> players, List<int> ignoreList)
        {
            var query = $@"INSERT INTO [dbo].WeeklyRedZoneRB (Season, Week, PlayerId, Name, Yardline, RushingAtt, RushingYds, RushingTD, Receptions, Targets, Yards, ReceivingTD, Fumbles)
                            VALUES(@Season, @Week, @PlayerId, @Name, @Yardline, @RushingAtt, @RushingYds, @RushingTD, @Receptions, @Targets, @Yards, @ReceivingTD, @Fumbles)";
            var count = 0;
            foreach (var player in players)
            {
                count += !ignoreList.Contains(player.PlayerId) ? await dbConnection.ExecuteAsync(query, player) : 0;
            }
            return count;
        }
    }
}
