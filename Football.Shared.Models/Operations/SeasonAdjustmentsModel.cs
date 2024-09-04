namespace Football.Shared.Models.Operations
{
    public class SeasonAdjustmentsModel
    {
        public bool InjuryAdjustment { get; set; }
        public bool SuspensionAdjustment { get; set; }
        public bool VeteranQBonNewTeamAdjustment { get; set; }
        public bool DownwardTrendingAdjustment { get; set; }
        public bool SharedBackfieldAdjustment { get; set; }
        public bool QuarterbackChangeAdjustment { get; set; }
        public bool LeadRBPromotionAdjustment { get; set; }
        public bool EliteRookieWRTopTargetAdjustment { get; set; }
        public bool PreviousSeasonBackupQuarterbackAdjustment { get; set; }
        public bool SharedReceivingDutiesAdjustment { get; set; }
    }
}
