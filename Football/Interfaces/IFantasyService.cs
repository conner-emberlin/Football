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
        public double CalculateTotalPoints(int playerId, int season);
        public double CalculatePassingPoints(int playerId, int season);
        public double CalculateRushingPoints(int playerId, int season);
        public double CalculateReceivingPoints(int playerId, int season);
        public FantasyPoints GetFantasyPoints(int playerId, int season);
        public int InsertFantasyPoints(FantasyPoints fantasyPoints);
        public List<int> GetPlayerIdsByFantasySeason(int season);
        public string GetPlayerPosition(int playerId);
        public List<int> GetPlayersByPosition(string position);
        public FantasyPoints GetFantasyResults(int playerId, int season);
        public (int, int) RefreshFantasyResults(FantasyPoints fantasyPoints);
        public List<int> GetActiveSeasons(int playerId);
        public double GetAverageTotalGames(int playerId);
        public List<int> GetActivePassingSeasons(int playerId);
        public List<int> GetActiveRushingSeasons(int playerId);
        public List<int> GetActiveReceivingSeasons(int playerId);
        public string GetPlayerName(int playerId);
        public bool IsPlayerActive(int playerId);
        public string GetPlayerTeam(int playerId);

    }
}
