using Dapper;
using Football.Models;
using System.Data;

namespace Football
{
    public class SettingsRepository(IDbConnection dbConnection) : ISettingsRepository
    {
        public async Task<bool> UploadSeasonTunings(Tunings tunings)
        {
            _ = await DeleteSeasonTunings(tunings.Season);

            var query = $@"INSERT INTO [dbo].Tunings (Season,	
                            RBFloor, LeadRBFactor, QBWeight, Weight, SecondYearWRLeap, SecondYearRBLeap, SecondYearQBLeap, SecondYearTELeap, NewQBFloor, NewQBCeiling, SeasonDataTrimmingGames,
                            AverageQBProjection, NewWRMinPoints, NewWRAdjustmentFactor, ExistingWRAdjustmentFactor, NewRBMinPoints, NewRBAdjustmentFactor, ExistingRBAdjustmentFactor, RushingQBThreshold,
                            ReceivingRBThreshold, BackupQBAdjustmentMax, VetQBNewTeamYears, VetQBNewTeamFactor, EliteWRDraftPositionMax, EliteWRRookieTopReceiverFactor, WR1MinPoints, RB1MinPoints,  
                            RBPromotionMinYardsPerCarry, RBPromotionFactor, NewQBCeilingForRB, MinGamesForMissedAverage, ReplacementLevelQB, ReplacementLevelRB, ReplacementLevelWR, ReplacementLevelTE )
                           VALUES (@Season,	
                            @RBFloor, @LeadRBFactor, @QBWeight, @Weight, @SecondYearWRLeap, @SecondYearRBLeap, @SecondYearQBLeap, @SecondYearTELeap, @NewQBFloor, @NewQBCeiling, @SeasonDataTrimmingGames,
                            @AverageQBProjection, @NewWRMinPoints, @NewWRAdjustmentFactor, @ExistingWRAdjustmentFactor, @NewRBMinPoints, @NewRBAdjustmentFactor, @ExistingRBAdjustmentFactor, @RushingQBThreshold,
                            @ReceivingRBThreshold, @BackupQBAdjustmentMax, @VetQBNewTeamYears, @VetQBNewTeamFactor, @EliteWRDraftPositionMax, @EliteWRRookieTopReceiverFactor, @WR1MinPoints, @RB1MinPoints,  
                            @RBPromotionMinYardsPerCarry, @RBPromotionFactor, @NewQBCeilingForRB, @MinGamesForMissedAverage, ReplacementLevelQB, ReplacementLevelRB, ReplacementLevelWR, ReplacementLevelTE
                            )";
            return (await dbConnection.ExecuteAsync(query, tunings)) > 0;
        }
        public async Task<bool> UploadWeeklyTunings(WeeklyTunings tunings)
        {
            var query = $@"INSERT INTO [dbo].WeeklyTunings Season, Week, RecentWeekWeight, ProjectionWeight, TamperedMin, TamperedMax, MinWeekWeighted, RecentWeeks, ErrorAdjustmentWeek)
                            VALUES (@Season, @Week, @RecentWeekWeight, @ProjectionWeight, @TamperedMin, @TamperedMax, @MinWeekWeighted, @RecentWeeks, @ErrorAdjustmentWeek)";
            return (await dbConnection.ExecuteAsync(query, tunings)) > 0;
        }

        public async Task<Tunings> GetSeasonTunings(int season)
        {
            var query = $@"SELECT * FROM [dbo].Tunings WHERE [Season] = @season";
            return (await dbConnection.QueryAsync<Tunings>(query, new { season })).First();
        }

        public async Task<WeeklyTunings> GetWeeklyTunings(int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].WeeklyTunings WHERE [Season] = @season AND [Week] <= @week
                        ORDER BY [Week] DESC";
            return (await dbConnection.QueryAsync<WeeklyTunings>(query, new { season, week })).First();
        }

        private async Task<int> DeleteSeasonTunings(int season)
        {
            var query = $@"DELETE FROM [dbo].Tunings WHERE [Season] = @season";
            return await dbConnection.ExecuteAsync(query, new { season });
        }
        private async Task<int> DeleteWeeklyTunings(int season, int week)
        {
            var query = $@"DELETE FROM [dbo].WeeklyTunings WHERE [Season] = @season AND [Week] = @week";
            return await dbConnection.ExecuteAsync(query, new { season, week });
        }
    }
}
