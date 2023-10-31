using System.Text.Json.Serialization;

namespace Football.Leagues.Models
{
    public class SleeperLeague
    {
        [JsonPropertyName("last_transaction_id")]
        public long? LastTransactionId { get; set; }

        [JsonPropertyName("total_rosters")]
        public int TotalRosters { get; set; }

        [JsonPropertyName("loser_bracket_id")]
        public object LoserBracketId { get; set; }

        [JsonPropertyName("group_id")]
        public object GroupId { get; set; }

        [JsonPropertyName("bracket_id")]
        public object BracketId { get; set; }

        [JsonPropertyName("roster_positions")]
        public List<string> RosterPositions { get; set; }

        [JsonPropertyName("previous_league_id")]
        public string PreviousLeagueId { get; set; }

        [JsonPropertyName("league_id")]
        public string LeagueId { get; set; }

        [JsonPropertyName("draft_id")]
        public string DraftId { get; set; }

        [JsonPropertyName("last_read_id")]
        public string LastReadId { get; set; }

        [JsonPropertyName("last_pinned_message_id")]
        public object LastPinnedMessageId { get; set; }

        [JsonPropertyName("last_message_time")]
        public object LastMessageTime { get; set; }

        [JsonPropertyName("last_message_text_map")]
        public object LastMessageTextMap { get; set; }

        [JsonPropertyName("last_message_attachment")]
        public object LastMessageAttachment { get; set; }

        [JsonPropertyName("last_author_is_bot")]
        public bool LastAuthorIsBot { get; set; }

        [JsonPropertyName("last_author_id")]
        public string LastAuthorId { get; set; }

        [JsonPropertyName("last_author_display_name")]
        public string LastAuthorDisplayName { get; set; }

        [JsonPropertyName("last_author_avatar")]
        public object LastAuthorAvatar { get; set; }

        [JsonPropertyName("display_order")]
        public int DisplayOrder { get; set; }

        [JsonPropertyName("avatar")]
        public object Avatar { get; set; }

        [JsonPropertyName("company_id")]
        public object CompanyId { get; set; }

        [JsonPropertyName("last_message_id")]
        public string LastMessageId { get; set; }

        [JsonPropertyName("scoring_settings")]
        public ScoringSettings ScoringSettings { get; set; }

        [JsonPropertyName("sport")]
        public string Sport { get; set; }

        [JsonPropertyName("season_type")]
        public string SeasonType { get; set; }

        [JsonPropertyName("season")]
        public string Season { get; set; }

        [JsonPropertyName("shard")]
        public int Shard { get; set; }

        [JsonPropertyName("settings")]
        public Settings Settings { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class ScoringSettings
    {
        [JsonPropertyName("st_ff")]
        public double StFf { get; set; }

        [JsonPropertyName("pts_allow_7_13")]
        public double PtsAllow713 { get; set; }

        [JsonPropertyName("def_st_ff")]
        public double DefStFf { get; set; }

        [JsonPropertyName("rec_yd")]
        public double RecYd { get; set; }

        [JsonPropertyName("fum_rec_td")]
        public double FumRecTd { get; set; }

        [JsonPropertyName("pts_allow_35p")]
        public double PtsAllow35p { get; set; }

        [JsonPropertyName("pts_allow_28_34")]
        public double PtsAllow2834 { get; set; }

        [JsonPropertyName("fum")]
        public double Fum { get; set; }

        [JsonPropertyName("rush_yd")]
        public double RushYd { get; set; }

        [JsonPropertyName("pass_td")]
        public double PassTd { get; set; }

        [JsonPropertyName("blk_kick")]
        public double BlkKick { get; set; }

        [JsonPropertyName("pass_yd")]
        public double PassYd { get; set; }

        [JsonPropertyName("safe")]
        public double Safe { get; set; }

        [JsonPropertyName("def_td")]
        public double DefTd { get; set; }

        [JsonPropertyName("fgm_50p")]
        public double Fgm50p { get; set; }

        [JsonPropertyName("def_st_td")]
        public double DefStTd { get; set; }

        [JsonPropertyName("fum_rec")]
        public double FumRec { get; set; }

        [JsonPropertyName("rush_2pt")]
        public double Rush2pt { get; set; }

        [JsonPropertyName("xpm")]
        public double Xpm { get; set; }

        [JsonPropertyName("pts_allow_21_27")]
        public double PtsAllow2127 { get; set; }

        [JsonPropertyName("fgm_20_29")]
        public double Fgm2029 { get; set; }

        [JsonPropertyName("pts_allow_1_6")]
        public double PtsAllow16 { get; set; }

        [JsonPropertyName("fum_lost")]
        public double FumLost { get; set; }

        [JsonPropertyName("def_st_fum_rec")]
        public double DefStFumRec { get; set; }

        [JsonPropertyName("int")]
        public double Int { get; set; }

        [JsonPropertyName("fgm_0_19")]
        public double Fgm019 { get; set; }

        [JsonPropertyName("pts_allow_14_20")]
        public double PtsAllow1420 { get; set; }

        [JsonPropertyName("rec")]
        public double Rec { get; set; }

        [JsonPropertyName("ff")]
        public double Ff { get; set; }

        [JsonPropertyName("fgmiss")]
        public double Fgmiss { get; set; }

        [JsonPropertyName("st_fum_rec")]
        public double StFumRec { get; set; }

        [JsonPropertyName("rec_2pt")]
        public double Rec2pt { get; set; }

        [JsonPropertyName("rush_td")]
        public double RushTd { get; set; }

        [JsonPropertyName("xpmiss")]
        public double Xpmiss { get; set; }

        [JsonPropertyName("fgm_30_39")]
        public double Fgm3039 { get; set; }

        [JsonPropertyName("rec_td")]
        public double RecTd { get; set; }

        [JsonPropertyName("st_td")]
        public double StTd { get; set; }

        [JsonPropertyName("pass_2pt")]
        public double Pass2pt { get; set; }

        [JsonPropertyName("pts_allow_0")]
        public double PtsAllow0 { get; set; }

        [JsonPropertyName("pass_int")]
        public double PassInt { get; set; }

        [JsonPropertyName("fgm_40_49")]
        public double Fgm4049 { get; set; }

        [JsonPropertyName("sack")]
        public double Sack { get; set; }

        [JsonPropertyName("fum_ret_yd")]
        public double? FumRetYd { get; set; }

        [JsonPropertyName("yds_allow_0_100")]
        public double? YdsAllow0100 { get; set; }

        [JsonPropertyName("int_ret_yd")]
        public double? IntRetYd { get; set; }

        [JsonPropertyName("pass_int_td")]
        public double? PassIntTd { get; set; }

        [JsonPropertyName("def_kr_yd")]
        public double? DefKrYd { get; set; }

        [JsonPropertyName("def_pr_yd")]
        public double? DefPrYd { get; set; }
    }

    public class Settings
    {
        [JsonPropertyName("daily_waivers_last_ran")]
        public int DailyWaiversLastRan { get; set; }

        [JsonPropertyName("reserve_allow_cov")]
        public int ReserveAllowCov { get; set; }

        [JsonPropertyName("reserve_slots")]
        public int ReserveSlots { get; set; }

        [JsonPropertyName("leg")]
        public int Leg { get; set; }

        [JsonPropertyName("offseason_adds")]
        public int OffseasonAdds { get; set; }

        [JsonPropertyName("bench_lock")]
        public int BenchLock { get; set; }

        [JsonPropertyName("trade_review_days")]
        public int TradeReviewDays { get; set; }

        [JsonPropertyName("league_average_match")]
        public int LeagueAverageMatch { get; set; }

        [JsonPropertyName("waiver_type")]
        public int WaiverType { get; set; }

        [JsonPropertyName("max_keepers")]
        public int MaxKeepers { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("pick_trading")]
        public int PickTrading { get; set; }

        [JsonPropertyName("disable_trades")]
        public int DisableTrades { get; set; }

        [JsonPropertyName("daily_waivers")]
        public int DailyWaivers { get; set; }

        [JsonPropertyName("taxi_years")]
        public int TaxiYears { get; set; }

        [JsonPropertyName("trade_deadline")]
        public int TradeDeadline { get; set; }

        [JsonPropertyName("reserve_allow_sus")]
        public int ReserveAllowSus { get; set; }

        [JsonPropertyName("reserve_allow_out")]
        public int ReserveAllowOut { get; set; }

        [JsonPropertyName("playoff_round_type")]
        public int PlayoffRoundType { get; set; }

        [JsonPropertyName("waiver_day_of_week")]
        public int WaiverDayOfWeek { get; set; }

        [JsonPropertyName("taxi_allow_vets")]
        public int TaxiAllowVets { get; set; }

        [JsonPropertyName("reserve_allow_dnr")]
        public int ReserveAllowDnr { get; set; }

        [JsonPropertyName("commissioner_direct_invite")]
        public int CommissionerDirectInvite { get; set; }

        [JsonPropertyName("reserve_allow_doubtful")]
        public int ReserveAllowDoubtful { get; set; }

        [JsonPropertyName("waiver_clear_days")]
        public int WaiverClearDays { get; set; }

        [JsonPropertyName("playoff_week_start")]
        public int PlayoffWeekStart { get; set; }

        [JsonPropertyName("daily_waivers_days")]
        public int DailyWaiversDays { get; set; }

        [JsonPropertyName("last_scored_leg")]
        public int LastScoredLeg { get; set; }

        [JsonPropertyName("taxi_slots")]
        public int TaxiSlots { get; set; }

        [JsonPropertyName("playoff_type")]
        public int PlayoffType { get; set; }

        [JsonPropertyName("daily_waivers_hour")]
        public int DailyWaiversHour { get; set; }

        [JsonPropertyName("num_teams")]
        public int NumTeams { get; set; }

        [JsonPropertyName("playoff_teams")]
        public int PlayoffTeams { get; set; }

        [JsonPropertyName("playoff_seed_type")]
        public int PlayoffSeedType { get; set; }

        [JsonPropertyName("start_week")]
        public int StartWeek { get; set; }

        [JsonPropertyName("reserve_allow_na")]
        public int ReserveAllowNa { get; set; }

        [JsonPropertyName("draft_rounds")]
        public int DraftRounds { get; set; }

        [JsonPropertyName("taxi_deadline")]
        public int TaxiDeadline { get; set; }

        [JsonPropertyName("waiver_bid_min")]
        public int WaiverBidMin { get; set; }

        [JsonPropertyName("capacity_override")]
        public int CapacityOverride { get; set; }

        [JsonPropertyName("disable_adds")]
        public int DisableAdds { get; set; }

        [JsonPropertyName("waiver_budget")]
        public int WaiverBudget { get; set; }

        [JsonPropertyName("last_report")]
        public int LastReport { get; set; }

        [JsonPropertyName("best_ball")]
        public int BestBall { get; set; }

        [JsonPropertyName("squads")]
        public int? Squads { get; set; }

        [JsonPropertyName("position_limit_rb")]
        public int? PositionLimitRb { get; set; }
    }
    public class Metadata
    {
        [JsonPropertyName("keeper_deadline")]
        public string KeeperDeadline { get; set; }

        [JsonPropertyName("copy_from_league_id")]
        public string CopyFromLeagueId { get; set; }

        [JsonPropertyName("auto_continue")]
        public string AutoContinue { get; set; }
    }



}
