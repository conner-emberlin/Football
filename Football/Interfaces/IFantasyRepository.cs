using Football.Models;
using Football.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Interfaces
{
    public interface IFantasyRepository
    {
        public FantasyPassing GetFantasyPassing(int playerId, int season);
        public FantasyRushing GetFantasyRushing(int playerId, int season);
        public FantasyReceiving GetFantasyReceiving(int playerId, int season);
        public List<int> GetPlayers();
        public string GetPlayerPosition(int playerId);
        public List<int> GetPlayersByPosition(string position);
        public int InsertFantasyPoints(FantasyPoints fantasyPoints);
        public FantasyPoints GetFantasyResults(int playerId, int season);
        public List<int> GetPlayerIdsByFantasySeason(int season);
        public (int, int) RefreshFantasyResults(FantasyPoints fantasyPoints);
        public List<int> GetActiveSeasons(int playerId);
        public double GetAverageTotalGames(int playerId, string position);
        public List<int> GetActivePassingSeasons(int playerId);
        public List<int> GetActiveRushingSeasons(int playerId);
        public List<int> GetActiveReceivingSeasons(int playerId);
        public string GetPlayerName(int playerId);
        public bool IsPlayerActive(int playerId);
        public string GetPlayerTeam(int playerId);
    }
}
