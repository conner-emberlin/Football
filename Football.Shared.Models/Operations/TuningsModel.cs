namespace Football.Shared.Models.Operations
{
    public class TuningsModel
    {
        public int RBFloor { get; set; }
        public double LeadRBFactor { get; set; }
        public double QBWeight { get; set; }
        public double Weight { get; set; }
        public double SecondYearWRLeap { get; set; }
        public double SecondYearRBLeap { get; set; }
        public double SecondYearQBLeap { get; set; }
        public double SecondYearTELeap { get; set; }
        public double NewQBFloor { get; set; }
        public double NewQBCeiling { get; set; }
        public double NewQBCeilingForRB { get; set; }
        public int SeasonDataTrimmingGames { get; set; }
        public double AverageQBProjection { get; set; }
        public double AverageRBProjection { get; set; }
        public double AverageWRProjection { get; set; }
        public double AverageTEProjection { get; set; }
        public double AverageDSTProjection { get; set; }
        public double AverageKProjection { get; set; }
        public double NewWRMinPoints { get; set; }
        public double NewWRAdjustmentFactor { get; set; }
        public double ExistingWRAdjustmentFactor { get; set; }
        public double NewRBMinPoints { get; set; }
        public double NewRBAdjustmentFactor { get; set; }
        public double ExistingRBAdjustmentFactor { get; set; }
        public double RushingQBThreshold { get; set; }
        public double ReceivingRBThreshold { get; set; }
        public double BackupQBAdjustmentMax { get; set; }
        public double VetQBNewTeamYears { get; set; }
        public double VetQBNewTeamFactor { get; set; }
        public int EliteWRDraftPositionMax { get; set; }
        public double EliteWRRookieTopReceiverFactor { get; set; }
        public double WR1MinPoints { get; set; }
        public double RB1MinPoints { get; set; }
        public double RBPromotionMinYardsPerCarry { get; set; }
        public double RBPromotionFactor { get; set; }
        public int MinGamesForMissedAverage { get; set; }
        public int ReplacementLevelQB { get; set; }
        public int ReplacementLevel2QB { get; set; }
        public int ReplacementLevelRB { get; set; }
        public int ReplacementLevelWR { get; set; }
        public int ReplacementLevelTE { get; set; }
        public int QBProjectionCount { get; set; }
        public int RBProjectionCount { get; set; }
        public int WRProjectionCount { get; set; }
        public int TEProjectionCount { get; set; }

    }
}
