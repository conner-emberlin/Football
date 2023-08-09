using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Football.Models;


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
        public string GetQbGames();
        public string GetRbGames();
        public string GetPcGames();
        public string GetPlayerName();
        public string IsPlayerActive();
        public string GetPlayerTeam();
        public string GetTightEnds();
        public string InsertFantasyProjections();
    }
}
