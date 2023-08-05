using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Football.Models;
using Football.Repository;

namespace Football.Services
{
    public  class FantasyService
    {
        public double CalculateTotalPoints(int playerId, int season)
        {
            return CalculatePassingPoints(playerId, season) + CalculateRushingPoints(playerId, season)
                    + CalculateReceivingPoints(playerId, season);
        }

        public double CalculatePassingPoints(int playerId, int season)
        {
            FantasyRepository fantasyRepository = new();
            var fantasyPassing = fantasyRepository.GetFantasyPassing(playerId, season);
            return fantasyPassing != null ?
                fantasyPassing.Yards * 0.04 + fantasyPassing.Touchdowns * 6 - fantasyPassing.Interceptions * 2
                : 0;
        }

        public double CalculateRushingPoints(int playerId, int season)
        {
            FantasyRepository fantasyRepository = new();
            var fantasyRushing = fantasyRepository.GetFantasyRushing(playerId, season);
            return fantasyRushing != null ?
                fantasyRushing.Yards * 0.1 + fantasyRushing.Touchdowns * 6 - fantasyRushing.Fumbles * 2
                :0;
        }

        public double CalculateReceivingPoints(int playerId, int season)
        {
            FantasyRepository fantasyRepository = new();
            var fantasyReceiving = fantasyRepository.GetFantasyReceiving(playerId, season);
            return fantasyReceiving != null ?
                fantasyReceiving.Yards * 0.1 + fantasyReceiving.Touchdowns * 6 + fantasyReceiving.Receptions
                :0;
        }

        public FantasyPoints GetFantasyPoints(int playerId, int season)
        {
            return new FantasyPoints
            {
                Season = season,
                PlayerId = playerId,
                TotalPoints = Math.Round((double)CalculateTotalPoints(playerId, season)),
                PassingPoints =Math.Round((double)CalculatePassingPoints(playerId, season)),
                RushingPoints = Math.Round((double)CalculateRushingPoints(playerId, season)),
                ReceivingPoints = Math.Round((double)CalculateReceivingPoints(playerId, season))
            };
        }

        public int InsertFantasyPoints(FantasyPoints fantasyPoints)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.InsertFantasyPoints(fantasyPoints);
        }

        public List<int> GetPlayerIdsByFantasySeason(int season)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.GetPlayerIdsByFantasySeason(season);
        }

        public string GetPlayerPosition(int playerId)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.GetPlayerPosition(playerId);
        }

        public List<int> GetPlayersByPosition(string position)
        {
            FantasyRepository fantasyRepositoty = new();
                return fantasyRepositoty.GetPlayersByPosition(position);
        }

        public FantasyPoints GetFantasyResults(int playerId, int season)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.GetFantasyResults(playerId, season);
        }

        public (int,int) RefreshFantasyResults(FantasyPoints fantasyPoints)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.RefreshFantasyResults(fantasyPoints);
        }

        public List<int> GetActiveSeasons(int playerId)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.GetActiveSeasons(playerId);
        }

        public double GetAverageTotalGames(int playerId)
        {
            FantasyRepository fantasyRepository = new();
            var position = GetPlayerPosition(playerId);
            return fantasyRepository.GetAverageTotalGames(playerId, position);
        }

        public List<int> GetActivePassingSeasons(int playerId)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.GetActivePassingSeasons(playerId);             
        }

        public List<int> GetActiveRushingSeasons(int playerId)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.GetActiveRushingSeasons(playerId);
        }

        public List<int> GetActiveReceivingSeasons(int playerId)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.GetActiveReceivingSeasons(playerId);
        }

        public string GetPlayerName(int playerId)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.GetPlayerName(playerId);
        }

        public bool IsPlayerActive(int playerId)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.IsPlayerActive(playerId);
        }

        public string GetPlayerTeam(int playerId)
        {
            FantasyRepository fantasyRepository = new();
            return fantasyRepository.GetPlayerTeam(playerId);
        }
    }
}
