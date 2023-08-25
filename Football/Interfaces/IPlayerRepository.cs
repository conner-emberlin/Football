using Football.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Interfaces
{
    public interface IPlayerRepository
    {
        public Task<PassingStatistic> GetPassingStatistic(int playerId, int season);
        public Task<PassingStatisticWithSeason> GetPassingStatisticWithSeason(int playerId, int season);
        public Task<List<PassingStatisticWithSeason>> GetPassingStatisticsWithSeason(int playerId);
        public Task<RushingStatistic> GetRushingStatistic(int playerId, int season);
        public Task<RushingStatisticWithSeason> GetRushingStatisticWithSeason(int playerId, int season);
        public Task<List<RushingStatisticWithSeason>> GetRushingStatisticsWithSeason(int playerId);
        public Task<ReceivingStatistic> GetReceivingStatistic(int playerId, int season);
        public Task<ReceivingStatisticWithSeason> GetReceivingStatisticWithSeason(int playerId, int season);
        public Task<List<ReceivingStatisticWithSeason>> GetReceivingStatisticsWithSeason(int playerId);
        public Task<string> GetPlayerPosition(int playerId);
        public Task<List<int>> GetPlayersByPosition(string position);
        public Task<List<int>> GetPlayerIdsByFantasySeason(int season);
        public Task<List<int>> GetActiveSeasons(int playerId);
        public Task<List<FantasySeasonGames>> GetFantasySeasonGames(int playerId);
        public Task<List<int>> GetActivePassingSeasons(int playerId);
        public Task<List<int>> GetActiveRushingSeasons(int playerId);
        public Task<List<int>> GetActiveReceivingSeasons(int playerId);
        public Task<string> GetPlayerName(int playerId);
        public Task<bool> IsPlayerActive(int playerId);
        public Task<string> GetPlayerTeam(int playerId);
        public Task<List<int>> GetTightEnds();
        public Task<Player> GetPlayerInfo(int playerId);
        public Task<int> GetPlayerId(string name);
        public Task<int> AddPassingStat(PassingStatisticWithSeason pass, int playerId);
        public Task<int> DeletePassingStats(int playerId);
        public Task<int> AddRushingStat(RushingStatisticWithSeason rush, int playerId);
        public Task<int> DeleteRushingStats(int playerId);
        public Task<int> AddReceivingStat(ReceivingStatisticWithSeason rec, int playerId);
        public Task<int> DeleteReceivingStats(int playerId);
    }
}
