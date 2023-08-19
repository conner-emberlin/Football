using Football.Interfaces;
using Football.Models;

namespace Football.Services
{
    public class RegressionModelService : IRegressionModelService
    {
        private readonly IFantasyService _fantasyService;
        private readonly IPlayerService _playerService;
        public RegressionModelService(IFantasyService fantasyService, IPlayerService playerService)
        {
            _fantasyService = fantasyService;
            _playerService = playerService;
        }

        public async Task<RegressionModelQB> PopulateRegressionModelQB(int playerId, int season)
        {
            PassingStatistic? passingStat = await _playerService.GetPassingStatistic(playerId, season);
            RushingStatistic? rushingStat = await _playerService.GetRushingStatistic(playerId, season);
            var dataP = passingStat != null;
            var dataR = rushingStat != null;
            return new RegressionModelQB
            {
                PlayerId = playerId,
                Season = season,
                PassingAttemptsPerGame = dataP ? Math.Round((double)(passingStat.Attempts / passingStat.Games), 4) : 0,
                PassingYardsPerGame = dataP ? Math.Round((double)(passingStat.Yards / passingStat.Games), 4) : 0,
                PassingTouchdownsPerGame = dataP ? Math.Round((double)(passingStat.Touchdowns / passingStat.Games), 4) : 0,
                RushingAttemptsPerGame = dataR ? Math.Round((double)(rushingStat.RushAttempts / rushingStat.Games), 4) : 0,
                RushingYardsPerGame = dataR ? Math.Round((double)(rushingStat.Yards / rushingStat.Games), 4) : 0,
                RushingTouchdownsPerGame = dataR ? Math.Round((double)(rushingStat.Touchdowns / rushingStat.Games), 4) : 0,
                SackYardsPerGame = dataP ? Math.Round((double)(passingStat.SackYards / passingStat.Games), 4) : 0
            };
        }

        public async Task<RegressionModelRB> PopulateRegressionModelRb(int playerId, int season)
        {
            RushingStatistic? rushingStat = await _playerService.GetRushingStatistic(playerId, season);
            ReceivingStatistic? receivingStat = await _playerService.GetReceivingStatistic(playerId, season);
            var dataRush = rushingStat != null;
            var dataRec = receivingStat != null;
            return new RegressionModelRB
            {
                PlayerId = playerId,
                Season = season,
                Age = dataRush ? rushingStat.Age : 0,
                RushingAttemptsPerGame = dataRush ? Math.Round((double)(rushingStat.RushAttempts / rushingStat.Games), 4) : 0,
                RushingYardsPerGame = dataRush ? Math.Round((double)(rushingStat.Yards / rushingStat.Games), 4) : 0,
                RushingYardsPerAttempt = dataRush ? Math.Round((double)(rushingStat.Yards / rushingStat.RushAttempts), 4) : 0,
                RushingTouchdownsPerGame = dataRush ? Math.Round((double)(rushingStat.Touchdowns / rushingStat.Games), 4) : 0,
                ReceivingTouchdownsPerGame = dataRec ? Math.Round((double)(receivingStat.Touchdowns / receivingStat.Games), 4) : 0,
                ReceivingYardsPerGame = dataRec ? Math.Round((double)(receivingStat.Yards / receivingStat.Games), 4) : 0,
                ReceptionsPerGame = dataRec ? Math.Round((double)(receivingStat.Receptions / receivingStat.Games), 4) : 0
            };
        }

        public async Task<RegressionModelPassCatchers> PopulateRegressionModelPassCatchers(int playerId, int season)
        {
            ReceivingStatistic? receivingStat = await _playerService.GetReceivingStatistic(playerId, season);
            var data = receivingStat != null;
            return new RegressionModelPassCatchers
            {
                PlayerId = playerId,
                Season = season,
                TargetsPerGame = data ? Math.Round((double)(receivingStat.Targets / receivingStat.Games), 4) : 0,
                ReceptionsPerGame = data ? Math.Round((double)(receivingStat.Receptions / receivingStat.Games), 4) : 0,
                YardsPerGame = data ? Math.Round((double)(receivingStat.Yards / receivingStat.Games), 4) : 0,
                YardsPerReception = data ? Math.Round((double)receivingStat.Yards / receivingStat.Receptions, 4) : 0,
                TouchdownsPerGame = data ? Math.Round((double)(receivingStat.Touchdowns / receivingStat.Games), 4) : 0
            };
        }

        public async Task<List<FantasyPoints>> PopulateFantasyResults(int season, string position)
        {
            List<FantasyPoints> fantasyPoints = new();
            List<int> playerIds = new();
            var players = await _playerService.GetPlayerIdsByFantasySeason(season);
            foreach (var p in players)
            {
                var playerPosition = await _playerService.GetPlayerPosition(p);
                if (playerPosition == position)
                {
                    playerIds.Add(p);
                }
            }
            foreach (var p in playerIds)
            {
                var pts = await _fantasyService.GetFantasyResults(p, season);
                if (pts != null)
                {
                    fantasyPoints.Add(pts);
                }
            }
            return fantasyPoints;
        }




    }
}
