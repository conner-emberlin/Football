using Dapper;
using Football.Models;
using System.Data;

namespace Football
{
    public class SettingsRepository(IDbConnection dbConnection) : ISettingsRepository
    {
        public async Task<bool> UploadCurrentSeasonTunings(Tunings tunings)
        {
            _ = await DeleteCurrentSeasonTunings(tunings.Season);

            var query = $@"INSERT INTO [dbo].Tunings (Season,	
                            RBFloor, LeadRBFactor, QBWeight, Weight, SecondYearWRLeap, SecondYearRBLeap, SecondYearQBLeap, SecondYearTELeap, NewQBFloor, NewQBCeiling, SeasonDataTrimmingGames,
                            AverageQBProjection, NewWRMinPoints, NewWRAdjustmentFactor, ExistingWRAdjustmentFactor, NewRBMinPoints, NewRBAdjustmentFactor, ExistingRBAdjustmentFactor, RushingQBThreshold,
                            ReceivingRBThreshold, BackupQBAdjustmentMax, VetQBNewTeamYears, VetQBNewTeamFactor, EliteWRDraftPositionMax, EliteWRRookieTopReceiverFactor, WR1MinPoints, RB1MinPoints,  
                            RBPromotionMinYardsPerCarry, RBPromotionFactor, NewQBCeilingForRB )
                           VALUES (@Season,	
                            @RBFloor, @LeadRBFactor, @QBWeight, @Weight, @SecondYearWRLeap, @SecondYearRBLeap, @SecondYearQBLeap, @SecondYearTELeap, @NewQBFloor, @NewQBCeiling, @SeasonDataTrimmingGames,
                            @AverageQBProjection, @NewWRMinPoints, @NewWRAdjustmentFactor, @ExistingWRAdjustmentFactor, @NewRBMinPoints, @NewRBAdjustmentFactor, @ExistingRBAdjustmentFactor, @RushingQBThreshold,
                            @ReceivingRBThreshold, @BackupQBAdjustmentMax, @VetQBNewTeamYears, @VetQBNewTeamFactor, @EliteWRDraftPositionMax, @EliteWRRookieTopReceiverFactor, @WR1MinPoints, @RB1MinPoints,  
                            @RBPromotionMinYardsPerCarry, @RBPromotionFactor, @NewQBCeilingForRB
                            )";
            return (await dbConnection.ExecuteAsync(query, tunings)) > 0;
        }

        private async Task<int> DeleteCurrentSeasonTunings(int season)
        {
            var query = $@"DELETE FROM [dbo].Tunings WHERE [Season] = @season";
            return await dbConnection.ExecuteAsync(query, new { season });
        }
    }
}
