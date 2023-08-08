using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Football.Models;
using Football.Repository;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Interfaces
{
    public interface IFantasyService
    {
        public Task<double> CalculateTotalPoints(int playerId, int season);
        public Task<double>  CalculatePassingPoints(int playerId, int season);
        public Task<double> CalculateRushingPoints(int playerId, int season);
        public Task<double> CalculateReceivingPoints(int playerId, int season);
        public Task<FantasyPoints> GetFantasyPoints(int playerId, int season);
        public Task<int> InsertFantasyPoints(FantasyPoints fantasyPoints);
        public Task<List<int>> GetPlayerIdsByFantasySeason(int season);
        public Task<string> GetPlayerPosition(int playerId);
        public Task<List<int>> GetPlayersByPosition(string position);
        public Task<FantasyPoints> GetFantasyResults(int playerId, int season);
        public Task<(int, int)> RefreshFantasyResults(FantasyPoints fantasyPoints);
        public Task<List<int>> GetActiveSeasons(int playerId);
        public Task<List<FantasySeasonGames>> GetAverageTotalGames(int playerId);
        public Task<List<int>> GetActivePassingSeasons(int playerId);
        public Task<List<int>> GetActiveRushingSeasons(int playerId);
        public Task<List<int>> GetActiveReceivingSeasons(int playerId);
        public Task<string> GetPlayerName(int playerId);
        public Task<bool> IsPlayerActive(int playerId);
        public Task<string> GetPlayerTeam(int playerId);
        public Task<List<int>> GetTightEnds();
    }
}
