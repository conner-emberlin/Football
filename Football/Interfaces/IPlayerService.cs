using Football.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Interfaces
{
    public interface IPlayerService
    {
        public Task<Player> GetPlayer(int playerId); 
        public Task<List<int>> GetPlayerIdsByFantasySeason(int season);
        public Task<string> GetPlayerPosition(int playerId);
        public Task<List<int>> GetPlayersByPosition(string position);       
        public Task<List<int>> GetActiveSeasons(int playerId);
        public Task<List<FantasySeasonGames>> GetAverageTotalGames(int playerId);
        public Task<string> GetPlayerName(int playerId);
        public Task<bool> IsPlayerActive(int playerId);
        public Task<string> GetPlayerTeam(int playerId);
        public Task<List<int>> GetTightEnds();
    }
}
