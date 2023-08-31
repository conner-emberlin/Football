﻿using Football.Interfaces;
using Football.Models;
using Serilog;
using Microsoft.Extensions.Options;


namespace Football.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IFantasyService _fantasyService;
        private readonly ILogger _logger;
        private readonly Season _season;
        public PlayerService(IPlayerRepository playerRepository, IFantasyService fantasyService, 
            ILogger logger, IOptionsMonitor<Season> season)
        {
            _playerRepository = playerRepository;
            _fantasyService = fantasyService;   
            _logger = logger;
            _season = season.CurrentValue;
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            try
            {
                var player = await GetPlayerInfo(playerId);
                player.PassingStats = await GetPassingStatisticsWithSeason(playerId);
                player.RushingStats = await GetRushingStatisticsWithSeason(playerId);
                player.ReceivingStats = await GetReceivingStatisticsWithSeason(playerId);
                player.FantasyPoints = await _fantasyService.GetAllFantasyResults(playerId);
                player.FantasySeasonGames = await GetFantasySeasonGames(playerId);
                var tightends = await GetTightEnds();
                player.IsTightEnd = tightends.Contains(playerId);
                return player;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }

        }
        public async Task<List<int>> GetPlayerIdsByFantasySeason(int season)
        {
            return await _playerRepository.GetPlayerIdsByFantasySeason(season);
        }

        public async Task<string> GetPlayerPosition(int playerId)
        {
            return await _playerRepository.GetPlayerPosition(playerId);
        }

        public async Task<List<int>> GetPlayersByPosition(string position)
        {
            return await _playerRepository.GetPlayersByPosition(position);
        }

        public async Task<List<int>> GetActiveSeasons(int playerId)
        {
            return await _playerRepository.GetActiveSeasons(playerId);
        }

        public async Task<string> GetPlayerName(int playerId)
        {
            return await _playerRepository.GetPlayerName(playerId);
        }

        public async Task<bool> IsPlayerActive(int playerId)
        {
            return await _playerRepository.IsPlayerActive(playerId);
        }

        public async Task<string> GetPlayerTeam(int playerId)
        {
            try
            {
                var rookies = await GetCurrentRookies(_season.CurrentSeason);
                if (rookies.Select(r => r.PlayerId).Contains(playerId))
                {
                    return rookies.Where(p => p.PlayerId == playerId).ToList().FirstOrDefault().TeamDrafted;
                }
                else
                {
                    return await _playerRepository.GetPlayerTeam(playerId);
                }       
            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString() + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        public async Task<List<int>> GetTightEnds()
        {
            return await _playerRepository.GetTightEnds();
        }

        public async Task<int> GetPlayerId(string name)
        {
            return await _playerRepository.GetPlayerId(name);
        }
        public async Task<List<FantasySeasonGames>> GetFantasySeasonGames(int playerId)
        {
            var games = await _playerRepository.GetFantasySeasonGames(playerId);
            return games.OrderBy(s => s.Season).ToList();
        }
        public async Task<int> AddPassingStat(PassingStatisticWithSeason pass)
        {
            var playerId = await _playerRepository.GetPlayerId(pass.Name);
            return await _playerRepository.AddPassingStat(pass, playerId);
        }
        public async Task<int> AddRushingStat(RushingStatisticWithSeason rush)
        {
            var playerId = await _playerRepository.GetPlayerId(rush.Name);
            return await _playerRepository.AddRushingStat(rush, playerId);
        }
        public async Task<int> AddReceivingStat(ReceivingStatisticWithSeason rec)
        {
            var playerId = await _playerRepository.GetPlayerId(rec.Name);
            return await _playerRepository.AddReceivingStat(rec, playerId);
        }

        private async Task<Player> GetPlayerInfo(int playerId)
        {
            return await _playerRepository.GetPlayerInfo(playerId);
        }
        private async Task<List<PassingStatisticWithSeason>> GetPassingStatisticsWithSeason(int playerId)
        {
            var stats = await _playerRepository.GetPassingStatisticsWithSeason(playerId);
            return stats.OrderBy(s => s.Season).ToList();
        }
        private async Task<List<RushingStatisticWithSeason>> GetRushingStatisticsWithSeason(int playerId)
        {
            var stats = await _playerRepository.GetRushingStatisticsWithSeason(playerId);
            return stats.OrderBy(s => s.Season).ToList();
        }
        private async Task<List<ReceivingStatisticWithSeason>> GetReceivingStatisticsWithSeason(int playerId)
        {
            var stats = await _playerRepository.GetReceivingStatisticsWithSeason(playerId);
            return stats.OrderBy(s => s.Season).ToList();
        }

        public async Task<int> DeletePassingStats(int playerId)
        {
            return await _playerRepository.DeletePassingStats(playerId);
        }

        public async Task<int> DeleteRushingStats(int playerId)
        {
            return await _playerRepository.DeleteRushingStats(playerId);
        }
        public async Task<int> DeleteReceivingStats(int playerId)
        {
            return await _playerRepository.DeleteReceivingStats(playerId);
        }

        public async Task<int> CreatePlayer(string name, string position, int active)
        {
            return await _playerRepository.CreatePlayer(name, position, active);
        }
        public async Task<List<Rookie>> GetHistoricalRookies(int season, string position)
        {
            return await _playerRepository.GetHistoricalRookies(season, position);
        }
        public async Task<List<Rookie>> GetCurrentRookies(int season, string position)
        {
            return await _playerRepository.GetCurrentRookies(season, position);
        }
        public async Task<List<Rookie>> GetCurrentRookies(int season)
        {
            return await _playerRepository.GetCurrentRookies(season);
        }
    }
}
