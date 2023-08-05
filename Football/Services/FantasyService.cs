using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Football.Interfaces;
using Football.Models;
using Football.Repository;

namespace Football.Services
{
    public  class FantasyService : IFantasyService
    {
        private readonly List<int> previousSeasons = new List<int>{ 2018, 2019, 2020, 2021 };
        public readonly IFantasyRepository _fantasyRepository;

        public FantasyService(IFantasyRepository fantasyRepository)
        {
            _fantasyRepository = fantasyRepository;
        }
        public double CalculateTotalPoints(int playerId, int season)
        {
            return CalculatePassingPoints(playerId, season) + CalculateRushingPoints(playerId, season)
                    + CalculateReceivingPoints(playerId, season);
        }

        public double CalculatePassingPoints(int playerId, int season)
        {
            var fantasyPassing = _fantasyRepository.GetFantasyPassing(playerId, season);
            return fantasyPassing != null ?
                fantasyPassing.Yards * 0.04 + fantasyPassing.Touchdowns * 6 - fantasyPassing.Interceptions * 2
                : 0;
        }

        public double CalculateRushingPoints(int playerId, int season)
        {
            var fantasyRushing = _fantasyRepository.GetFantasyRushing(playerId, season);
            return fantasyRushing != null ?
                fantasyRushing.Yards * 0.1 + fantasyRushing.Touchdowns * 6 - fantasyRushing.Fumbles * 2
                :0;
        }

        public double CalculateReceivingPoints(int playerId, int season)
        {
            var fantasyReceiving = _fantasyRepository.GetFantasyReceiving(playerId, season);
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
            return _fantasyRepository.InsertFantasyPoints(fantasyPoints);
        }

        public List<int> GetPlayerIdsByFantasySeason(int season)
        {
            return _fantasyRepository.GetPlayerIdsByFantasySeason(season);
        }

        public string GetPlayerPosition(int playerId)
        {
            return _fantasyRepository.GetPlayerPosition(playerId);
        }

        public List<int> GetPlayersByPosition(string position)
        {
           return _fantasyRepository.GetPlayersByPosition(position);
        }

        public FantasyPoints GetFantasyResults(int playerId, int season)
        {
            return _fantasyRepository.GetFantasyResults(playerId, season);
        }

        public (int,int) RefreshFantasyResults(FantasyPoints fantasyPoints)
        {
            return _fantasyRepository.RefreshFantasyResults(fantasyPoints);
        }

        public List<int> GetActiveSeasons(int playerId)
        {
            return _fantasyRepository.GetActiveSeasons(playerId);
        }

        public double GetAverageTotalGames(int playerId)
        {
            var position = GetPlayerPosition(playerId);
            return _fantasyRepository.GetAverageTotalGames(playerId, position);
        }

        public List<int> GetActivePassingSeasons(int playerId)
        {
            return _fantasyRepository.GetActivePassingSeasons(playerId);             
        }

        public List<int> GetActiveRushingSeasons(int playerId)
        {
            return _fantasyRepository.GetActiveRushingSeasons(playerId);
        }

        public List<int> GetActiveReceivingSeasons(int playerId)
        {
            return _fantasyRepository.GetActiveReceivingSeasons(playerId);
        }

        public string GetPlayerName(int playerId)
        {
            return _fantasyRepository.GetPlayerName(playerId);
        }

        public bool IsPlayerActive(int playerId)
        {
            return _fantasyRepository.IsPlayerActive(playerId);
        }

        public string GetPlayerTeam(int playerId)
        {
            return _fantasyRepository.GetPlayerTeam(playerId);
        }
    }
}
