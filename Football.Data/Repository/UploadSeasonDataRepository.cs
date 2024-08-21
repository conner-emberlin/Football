using Football.Data.Interfaces;
using Football.Players.Models;
using System.Data;
using Dapper;

namespace Football.Data.Repository
{
    public class UploadSeasonDataRepository(IDbConnection dbConnection) : IUploadSeasonDataRepository
    {
        public async Task<int> UploadSeasonQBData(IEnumerable<SeasonDataQB> players)
        {
            var query = $@"INSERT INTO [dbo].SeasonQBData (Season, PlayerId, Name, Completions, Attempts, Yards, TD, Int, Sacks, RushingAttempts, RushingYards, RushingTD, Fumbles, Games)
                        VALUES (@Season, @PlayerId, @Name, @Completions, @Attempts, @Yards, @TD, @Int, @Sacks, @RushingAttempts, @RushingYards, @RushingTD, @Fumbles, @Games)";
            return await dbConnection.ExecuteAsync(query, players);
        }
        public async Task<int> UploadSeasonRBData(IEnumerable<SeasonDataRB> players)
        {
            var query = $@"INSERT INTO [dbo].SeasonRBData (Season, PlayerId, Name, RushingAtt, RushingYds, RushingTD, Receptions, Targets, Yards, ReceivingTD, Fumbles, Games)
                            VALUES(@Season, @PlayerId, @Name, @RushingAtt, @RushingYds, @RushingTD, @Receptions, @Targets, @Yards, @ReceivingTD, @Fumbles, @Games)";
            return await dbConnection.ExecuteAsync(query, players);

        }
        public async Task<int> UploadSeasonWRData(IEnumerable<SeasonDataWR> players)
        {
            var query = $@"INSERT INTO [dbo].SeasonWRData (Season, PlayerId, Name, Receptions, Targets, Yards, Long, TD, RushingAtt, RushingYds, RushingTD, Fumbles, Games)
                        VALUES (@Season, @PlayerId, @Name, @Receptions, @Targets, @Yards, @Long, @TD, @RushingAtt, @RushingYds, @RushingTD, @Fumbles, @Games)";
            return await dbConnection.ExecuteAsync(query, players);
        }
        public async Task<int> UploadSeasonTEData(IEnumerable<SeasonDataTE> players)
        {
            var query = $@"INSERT INTO [dbo].SeasonTEData (Season, PlayerId, Name, Receptions, Targets, Yards, Long, TD, RushingAtt, RushingYds, RushingTD, Fumbles, Games)
                        VALUES (@Season, @PlayerId, @Name, @Receptions, @Targets, @Yards, @Long, @TD, @RushingAtt, @RushingYds, @RushingTD, @Fumbles, @Games)";
            return await dbConnection.ExecuteAsync(query, players);

        }
        public async Task<int> UploadSeasonDSTData(List<SeasonDataDST> players)
        {
            var query = $@"INSERT INTO [dbo].SeasonDSTData (Season, PlayerId, Name, Sacks, Ints, FumblesRecovered, ForcedFumbles, DefensiveTD, Safties, SpecialTD, Games)
                        VALUES(@Season, @PlayerId, @Name, @Sacks, @Ints, @FumblesRecovered, @ForcedFumbles, @DefensiveTD, @Safties, @SpecialTD, @Games)";
            return await dbConnection.ExecuteAsync(query, players);
        }

        public async Task<int> UploadCurrentTeams(List<PlayerTeam> teams)
        {
            var query = $@"INSERT INTO [dbo].PlayerTeam (Season, PlayerId, Name, Team, TeamId)
                        VALUES (@Season, @PlayerId, @Name, @Team, @TeamId)";
            return await dbConnection.ExecuteAsync(query, teams);
        }
        public async Task<int> UploadSchedule(List<Schedule> schedules)
        {
            var query = $@"INSERT INTO [dbo].Schedule (Season, TeamId, Team, Week, OpposingTeamId, OpposingTeam)
                        VALUES (@Season, @TeamId, @Team, @Week, @OpposingTeamId, @OpposingTeam)";
            return await dbConnection.ExecuteAsync(query, schedules);
        }
        public async Task<int> UploadScheduleDetails(List<ScheduleDetails> scheduleDetails)
        {
            var query = $@"INSERT INTO [dbo].ScheduleDetails (Season, Week, Day, Date, Time, HomeTeamId, AwayTeamId)
                            VALUES (@Season, @Week, @Day, @Date, @Time, @HomeTeamId, @AwayTeamId)";
            return await dbConnection.ExecuteAsync(query, scheduleDetails);
        }
        public async Task<int> UploadADP(List<SeasonADP> adp)
        {
            var query = $@"INSERT INTO [dbo].ADP (Season, PlayerId, Name, Position, PositionADP, OverallADP)
                            VALUES (@Season, @PlayerId, @Name, @Position, @PositionADP, @OverallADP)";
            return await dbConnection.ExecuteAsync(query, adp);
        }

        public async Task<int> UploadConsensusProjections(List<ConsensusProjections> projections)
        {
            var query = $@"INSERT INTO [dbo].ConsensusProjections (PlayerId, Season, Position, FantasyPoints)
                        VALUES (@PlayerId, @Season, @Position, @FantasyPoints)";
            return await dbConnection.ExecuteAsync(query, projections);
        }




    }
}
