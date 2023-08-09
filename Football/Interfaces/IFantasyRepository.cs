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
        public Task<FantasyPassing> GetFantasyPassing(int playerId, int season);
        public Task<FantasyRushing>GetFantasyRushing(int playerId, int season);
        public Task<FantasyReceiving> GetFantasyReceiving(int playerId, int season);
        public Task<List<int>> GetPlayers();
        public Task<string> GetPlayerPosition(int playerId);
        public Task<List<int>> GetPlayersByPosition(string position);
        public Task<int> InsertFantasyPoints(FantasyPoints fantasyPoints);
        public Task<FantasyPoints> GetFantasyResults(int playerId, int season);
        public Task<List<int>> GetPlayerIdsByFantasySeason(int season);
        public Task<(int, int)> RefreshFantasyResults(FantasyPoints fantasyPoints);
        public Task<List<int>> GetActiveSeasons(int playerId);
        public Task<List<FantasySeasonGames>> GetAverageTotalGames(int playerId, string position);
        public Task<List<int>> GetActivePassingSeasons(int playerId);
        public Task<List<int>> GetActiveRushingSeasons(int playerId);
        public Task<List<int>> GetActiveReceivingSeasons(int playerId);
        public Task<string> GetPlayerName(int playerId);
        public Task<bool> IsPlayerActive(int playerId);
        public Task<string>GetPlayerTeam(int playerId);

        public Task<List<int>> GetTightEnds();
        public Task<int> InsertFantasyProjections(int rank, ProjectionModel proj);
    }
}
