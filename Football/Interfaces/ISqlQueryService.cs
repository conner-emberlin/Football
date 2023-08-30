namespace Football.Interfaces
{
    public interface ISqlQueryService
    {
        public string GetQueryString(string type);
        public string FantasyPassingQuery();
        public string FantasyRushingQuery();
        public string FantasyReceivingQuery();
        public string GetPlayerIds();
        public string InsertFantasyData();
        public string GetPlayerPosition();
        public string GetPlayersByPosition();
        public string GetPassingStatistic();
        public string GetPassingStatisticWithSeason();
        public string GetRushingStatistic();
        public string GetRushingStatisticWithSeason();
        public string GetReceivingStatistic();
        public string GetReceivingStatisticWithSeason();
        public string GetFantasyPoints();
        public string GetPlayerIdsByFantasySeason();
        public string DeleteFantasyPoints();
        public string GetSeasons();
        public string GetActivePassingSeasons();
        public string GetActiveRushingSeasons();
        public string GetActiveReceivingSeasons();
        public string GetFantasyGames();
        public string GetPlayerName();
        public string IsPlayerActive();
        public string GetPlayerTeam();
        public string GetTightEnds();
        public string InsertFantasyProjections();
        public string GetPlayerInfo();
        public string GetPassingStatisticsWithSeason();
        public string GetRushingStatisticsWithSeason();
        public string GetReceivingStatisticsWithSeason();
        public string GetAllFantasyResults();
        public string GetPlayerId();
        public string AddPassingStat();
        public string DeletePassingStats();
        public string AddRushingStat();
        public string DeleteRushingStats();
        public string AddReceivingStat();
        public string DeleteReceivingStats();
        public string GetGamesSuspended();
        public string GetTeamChange();
        public string GetPlayerTeamChange();
        public string CreatePlayer();
        public string GetHistoricalRookies();
        public string GetCurrentRookies();
        public string GetAllCurrentRookies();
        public string GetInjuryConcerns();
    }
}
