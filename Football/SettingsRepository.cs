using Dapper;
using Football.Models;
using System.Data;

namespace Football
{
    public class SettingsRepository(IDbConnection dbConnection) : ISettingsRepository
    {
        public async Task<bool> UploadSeasonTunings(Tunings tunings)
        {
            if ((await GetSeasonTunings(tunings.Season) != null))
            {
                _ = await DeleteSeasonTunings(tunings.Season);

                var query = $@"INSERT INTO [dbo].Tunings (Season,	
                            RBFloor, LeadRBFactor, QBWeight, Weight, SecondYearWRLeap, SecondYearRBLeap, SecondYearQBLeap, SecondYearTELeap, NewQBFloor, NewQBCeiling, SeasonDataTrimmingGames,
                            AverageQBProjection, AverageRBProjection, AverageWRProjection, AverageTEProjection, AverageDSTProjection, AverageKProjection, NewWRMinPoints, NewWRAdjustmentFactor, ExistingWRAdjustmentFactor, NewRBMinPoints, NewRBAdjustmentFactor, ExistingRBAdjustmentFactor, RushingQBThreshold,
                            ReceivingRBThreshold, BackupQBAdjustmentMax, VetQBNewTeamYears, VetQBNewTeamFactor, EliteWRDraftPositionMax, EliteWRRookieTopReceiverFactor, WR1MinPoints, RB1MinPoints,  
                            RBPromotionMinYardsPerCarry, RBPromotionFactor, NewQBCeilingForRB, MinGamesForMissedAverage, ReplacementLevelQB, ReplacementLevel2QB, ReplacementLevelRB, ReplacementLevelWR, ReplacementLevelTE,
                            QBProjectionCount, RBProjectionCount, WRProjectionCount, TEProjectionCount )
                           VALUES (@Season,	
                            @RBFloor, @LeadRBFactor, @QBWeight, @Weight, @SecondYearWRLeap, @SecondYearRBLeap, @SecondYearQBLeap, @SecondYearTELeap, @NewQBFloor, @NewQBCeiling, @SeasonDataTrimmingGames,
                            @AverageQBProjection, @AverageRBProjection, @AverageWRProjection, @AverageTEProjection, @AverageDSTProjection, @AverageKProjection, @NewWRMinPoints, @NewWRAdjustmentFactor, @ExistingWRAdjustmentFactor, @NewRBMinPoints, @NewRBAdjustmentFactor, @ExistingRBAdjustmentFactor, @RushingQBThreshold,
                            @ReceivingRBThreshold, @BackupQBAdjustmentMax, @VetQBNewTeamYears, @VetQBNewTeamFactor, @EliteWRDraftPositionMax, @EliteWRRookieTopReceiverFactor, @WR1MinPoints, @RB1MinPoints,  
                            @RBPromotionMinYardsPerCarry, @RBPromotionFactor, @NewQBCeilingForRB, @MinGamesForMissedAverage, @ReplacementLevelQB, @ReplacementLevel2QB, @ReplacementLevelRB, @ReplacementLevelWR, @ReplacementLevelTE,
                            @QBProjectionCount, @RBProjectionCount, @WRProjectionCount, @TEProjectionCount 
                            )";
                return (await dbConnection.ExecuteAsync(query, tunings)) > 0;
            }
            return false;   
        }
        public async Task<bool> UploadWeeklyTunings(WeeklyTunings tunings)
        {
            var query = $@"INSERT INTO [dbo].WeeklyTunings (Season, Week, RecentWeekWeight, ProjectionWeight, TamperedMin, TamperedMax, MinWeekWeighted, RecentWeeks, ErrorAdjustmentWeek,
                            QBProjectionCount, RBProjectionCount, WRProjectionCount, TEProjectionCount)
                            VALUES (@Season, @Week, @RecentWeekWeight, @ProjectionWeight, @TamperedMin, @TamperedMax, @MinWeekWeighted, @RecentWeeks, @ErrorAdjustmentWeek,
                            @QBProjectionCount, @RBProjectionCount, @WRProjectionCount, @TEProjectionCount )";
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

        public async Task<bool> UploadSeasonAdjustments(SeasonAdjustments adjustments)
        {
            _ = await DeleteSeasonAdjustments(adjustments.Season);

            var query = $@"INSERT INTO [dbo].SeasonAdjustments (
                            Season,
                            InjuryAdjustment,
                            SuspensionAdjustment,
                            VeteranQBonNewTeamAdjustment,
                            DownwardTrendingAdjustment,
                            SharedBackfieldAdjustment,
                            QuarterbackChangeAdjustment,
                            LeadRBPromotionAdjustment, 
                            EliteRookieWRTopTargetAdjustment, 
                            PreviousSeasonBackupQuarterbackAdjustment,
                            SharedReceivingDutiesAdjustment)
                        VALUES (
                            @Season,
                            @InjuryAdjustment,
                            @SuspensionAdjustment,
                            @VeteranQBonNewTeamAdjustment,
                            @DownwardTrendingAdjustment,
                            @SharedBackfieldAdjustment,
                            @QuarterbackChangeAdjustment,
                            @LeadRBPromotionAdjustment, 
                            @EliteRookieWRTopTargetAdjustment, 
                            @PreviousSeasonBackupQuarterbackAdjustment,
                            @SharedReceivingDutiesAdjustment)";
            return await dbConnection.ExecuteAsync(query, adjustments) > 0;
        }

        public async Task<SeasonAdjustments> GetSeasonAdjustments(int season)
        {
            var query = $@"SELECT * FROM [dbo].SeasonAdjustments WHERE [Season] = @season";
            return (await dbConnection.QueryAsync<SeasonAdjustments>(query, new { season })).First();
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
        private async Task<int> DeleteSeasonAdjustments(int season)
        {
            var query = $@"DELETE FROM [dbo].SeasonAdjustments WHERE [Season] = @season";
            return await dbConnection.ExecuteAsync(query, new { season });
        }
    }
}
