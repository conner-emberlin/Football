using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using System.Text;
using System.Threading.Tasks;
using Football.Models;
using Football.Repository;
using Football.Interfaces;

namespace Football.Services
{
    public class RegressionModelService : IRegressionModelService
    {
        public RegressionModelQB PopulateRegressionModelQB(int playerId, int season)
        {
            PassingStatistic? passingStat = GetPassingStatistic(playerId, season);
            RushingStatistic? rushingStat = GetRushingStatistic(playerId, season);
            var dataP = passingStat != null;
            var dataR = rushingStat != null;
            return new RegressionModelQB
            {
                PlayerId = playerId,
                Season = season,
                PassingAttemptsPerGame = dataP ? Math.Round((double)(passingStat.Attempts / passingStat.Games),4) : 0,
                PassingYardsPerGame = dataP ? Math.Round((double)(passingStat.Yards / passingStat.Games),4) : 0,
                PassingTouchdownsPerGame = dataP ? Math.Round((double)(passingStat.Touchdowns / passingStat.Games), 4) : 0,
                RushingAttemptsPerGame = dataR ? Math.Round((double)(rushingStat.RushAttempts / rushingStat.Games),4) : 0,
                RushingYardsPerGame = dataR ? Math.Round((double)(rushingStat.Yards / rushingStat.Games),4) : 0,
                RushingTouchdownsPerGame = dataR ? Math.Round((double)(rushingStat.Touchdowns/rushingStat.Games), 4) : 0,
                SackYardsPerGame = dataP ? Math.Round((double)(passingStat.SackYards / passingStat.Games), 4) : 0
            };         
        }

        public RegressionModelRB PopulateRegressionModelRb(int playerId, int season)
        { 
            RushingStatistic? rushingStat = GetRushingStatistic(playerId, season);
            ReceivingStatistic? receivingStat = GetReceivingStatistic(playerId, season);
            var dataRush = rushingStat != null;
            var dataRec = receivingStat != null;
            return new RegressionModelRB
            {
                PlayerId = playerId,
                Season = season,
                RushingAttemptsPerGame = dataRush ? Math.Round((double)(rushingStat.RushAttempts / rushingStat.Games),4) : 0,
                RushingYardsPerGame = dataRush ? Math.Round((double)(rushingStat.Yards / rushingStat.Games),4) : 0,
                RushingYardsPerAttempt = dataRush ? Math.Round((double)(rushingStat.Yards / rushingStat.RushAttempts),4) : 0,
                RushingTouchdownsPerGame = dataRush ? Math.Round((double)(rushingStat.Touchdowns / rushingStat.Games), 4) : 0,
                ReceivingTouchdownsPerGame = dataRec ? Math.Round((double)(receivingStat.Touchdowns / receivingStat.Games),4) : 0,
                ReceivingYardsPerGame = dataRec ? Math.Round((double)(receivingStat.Yards / receivingStat.Games), 4) : 0,
                ReceptionsPerGame = dataRec ? Math.Round((double)(receivingStat.Receptions / receivingStat.Games), 4) : 0
            };
        }

        public RegressionModelPassCatchers PopulateRegressionModelPassCatchers(int playerId, int season)
        {
            ReceivingStatistic? receivingStat = GetReceivingStatistic(playerId,season);
            var data = receivingStat != null;
            return new RegressionModelPassCatchers
            {
                PlayerId = playerId,
                Season = season,
                TargetsPerGame = data ? Math.Round((double)(receivingStat.Targets / receivingStat.Games),4) : 0,
                ReceptionsPerGame = data ? Math.Round((double)(receivingStat.Receptions / receivingStat.Games), 4) : 0,
                YardsPerGame = data ? Math.Round((double)(receivingStat.Yards / receivingStat.Games), 4) : 0,
                YardsPerReception = data ? Math.Round((double)receivingStat.Yards/receivingStat.Receptions, 4) : 0,
                TouchdownsPerGame = data ?  Math.Round((double)(receivingStat.Touchdowns / receivingStat.Games), 4) : 0
            };
        }

        public List<FantasyPoints> PopulateFantasyResults(int season, string position)
        {
            FantasyService fantasyService = new();
            List<FantasyPoints> fantasyPoints = new();
            List<int> playerIds = fantasyService.GetPlayerIdsByFantasySeason(season).Where(x => fantasyService.GetPlayerPosition(x) == position).ToList();
            foreach (var p in playerIds)
            {
                var pts = fantasyService.GetFantasyResults(p, season);
                if (pts != null)
                {
                    fantasyPoints.Add(pts);
                }
            }
            return fantasyPoints;
        }

        public PassingStatistic GetPassingStatistic(int playerId, int season)
        {
            RegressionModelRepository regressionModelRepository = new();
            return regressionModelRepository.GetPassingStatistic(playerId, season);
        }

        public RushingStatistic GetRushingStatistic(int playerId, int season)
        {
            RegressionModelRepository regressionModelRepository = new();
            return regressionModelRepository.GetRushingStatistic(playerId, season);
        }

        public ReceivingStatistic GetReceivingStatistic(int playerId, int season)
        {
            RegressionModelRepository regressionModelRepository = new();
            return regressionModelRepository.GetReceivingStatistic(playerId, season);
        }
    }
}
