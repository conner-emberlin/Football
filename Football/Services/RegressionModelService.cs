using Football.Interfaces;
using Football.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System.Numerics;

namespace Football.Services
{
    public class RegressionModelService : IRegressionModelService
    {
        private readonly IFantasyService _fantasyService;
        private readonly IPlayerService _playerService;
        private readonly ILogger _logger;
        private readonly Season _season;
        public RegressionModelService(IFantasyService fantasyService, IPlayerService playerService, 
            ILogger logger, IOptionsMonitor<Season> season)
        {
            _fantasyService = fantasyService;
            _playerService = playerService;
            _logger = logger;
            _season = season.CurrentValue;
        }

        public RegressionModelQB RegressionModelQB(Player player, int season)
        {
            try
            {
                var passingStat = player.PassingStats?.Where(p => p.Season == season).FirstOrDefault();
                var rushingStat = player.RushingStats?.Where(p => p.Season == season).FirstOrDefault();
                var dataP = passingStat != null;
                var dataR = rushingStat != null;
                return new RegressionModelQB
                {
                    PlayerId = player.PlayerId,
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
            catch(Exception ex) 
            { 
                _logger.Error(ex.Message, ex);
                _logger.Information("Check out this player " + player.PlayerId);
                throw;
            }
        }

        public RegressionModelRB RegressionModelRB(Player player, int season)
        {
            try
            {
                var rushingStat = player.RushingStats?.Where(p => p.Season == season).FirstOrDefault();
                var receivingStat = player.ReceivingStats?.Where(p => p.Season == season).FirstOrDefault();
                var dataRush = rushingStat != null;
                var dataRec = receivingStat != null;
                return new RegressionModelRB
                {
                    PlayerId = player.PlayerId,
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
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                _logger.Information("Check out this player " + player.PlayerId);
                throw;
            }
        }
        public RegressionModelPassCatchers RegressionModelPC(Player player, int season)
        {
            try
            {
                var receivingStat = player.ReceivingStats?.Where(p => p.Season == season).FirstOrDefault();
                var data = receivingStat != null;
                return new RegressionModelPassCatchers
                {
                    PlayerId = player.PlayerId,
                    Season = season,
                    Age = data ? receivingStat.Age : 0,
                    TargetsPerGame = data ? Math.Round((double)(receivingStat.Targets / receivingStat.Games), 4) : 0,
                    ReceptionsPerGame = data ? Math.Round((double)(receivingStat.Receptions / receivingStat.Games), 4) : 0,
                    YardsPerGame = data ? Math.Round((double)(receivingStat.Yards / receivingStat.Games), 4) : 0,
                    YardsPerReception = data ? Math.Round((double)receivingStat.Yards / receivingStat.Receptions, 4) : 0,
                    TouchdownsPerGame = data ? Math.Round((double)(receivingStat.Touchdowns / receivingStat.Games), 4) : 0
                };
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                _logger.Information("Check out this player " + player.PlayerId);
                throw;
            }
        }
        public RegressionModelQB RegressionModelQB(PassingStatistic passingStat, RushingStatistic rushingStat, int playerId)
        {
            var dataP = passingStat != null;
            var dataR = rushingStat != null;
            try
            {
                return new RegressionModelQB
                {
                    PlayerId = playerId,
                    Season = _season.CurrentSeason,
                    PassingAttemptsPerGame = dataP ? Math.Round((double)(passingStat.Attempts / passingStat.Games), 4) : 0,
                    PassingYardsPerGame = dataP ? Math.Round((double)(passingStat.Yards / passingStat.Games), 4) : 0,
                    PassingTouchdownsPerGame = dataP ? Math.Round((double)(passingStat.Touchdowns / passingStat.Games), 4) : 0,
                    RushingAttemptsPerGame = dataR ? Math.Round((double)(rushingStat.RushAttempts / rushingStat.Games), 4) : 0,
                    RushingYardsPerGame = dataR ? Math.Round((double)(rushingStat.Yards / rushingStat.Games), 4) : 0,
                    RushingTouchdownsPerGame = dataR ? Math.Round((double)(rushingStat.Touchdowns / rushingStat.Games), 4) : 0,
                    SackYardsPerGame = dataP ? Math.Round((double)(passingStat.SackYards / passingStat.Games), 4) : 0
                };
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                _logger.Information("Check out this player " + playerId);
                throw;
            }
        }

        public RegressionModelRB RegressionModelRB(RushingStatistic rushingStat, ReceivingStatistic receivingStat, int playerId)
        {
            var dataRush = rushingStat != null;
            var dataRec = receivingStat != null;
            try
            {
                return new RegressionModelRB
                {
                    PlayerId = playerId,
                    Season = _season.CurrentSeason,
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
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                _logger.Information("Check out this player " + playerId);
                throw;
            }
        }

        public RegressionModelPassCatchers RegressionModelPC(ReceivingStatistic receivingStat, int playerId)
        {
            try
            {
                var data = receivingStat != null;
                return new RegressionModelPassCatchers
                {
                    PlayerId = playerId,
                    Season = _season.CurrentSeason,
                    Age = data ? receivingStat.Age : 0,
                    TargetsPerGame = data ? Math.Round((double)(receivingStat.Targets / receivingStat.Games), 4) : 0,
                    ReceptionsPerGame = data ? Math.Round((double)(receivingStat.Receptions / receivingStat.Games), 4) : 0,
                    YardsPerGame = data ? Math.Round((double)(receivingStat.Yards / receivingStat.Games), 4) : 0,
                    YardsPerReception = data ? Math.Round((double)receivingStat.Yards / receivingStat.Receptions, 4) : 0,
                    TouchdownsPerGame = data ? Math.Round((double)(receivingStat.Touchdowns / receivingStat.Games), 4) : 0
                };
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                _logger.Information("Check out this player " + playerId);
                throw;
            }
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
