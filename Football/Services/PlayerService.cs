using Football.Interfaces;
using Football.Models;
using Football.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IFantasyService _fantasyService;

        public PlayerService(IPlayerRepository playerRepository, IFantasyService fantasyService)
        {
            _playerRepository = playerRepository;
            _fantasyService = fantasyService;   
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            var player = await GetPlayerInfo(playerId);
            player.PassingStats = await GetPassingStatisticsWithSeason(playerId);
            player.RushingStats = await GetRushingStatisticsWithSeason(playerId);
            player.ReceivingStats = await GetReceivingStatisticsWithSeason(playerId);
            player.FantasyPoints = await _fantasyService.GetAllFantasyResults(playerId);
            player.FantasySeasonGames = await GetAverageTotalGames(playerId);
            
            var tightends = await GetTightEnds();
            if (tightends.Contains(playerId))
            {
                player.IsTightEnd = true;
            }
            else { 
                player.IsTightEnd = false; 
            }
            
            return player;

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

        public async Task<List<FantasySeasonGames>> GetAverageTotalGames(int playerId)
        {
            var position = await GetPlayerPosition(playerId);
            return await _playerRepository.GetAverageTotalGames(playerId, position);
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
            return await _playerRepository.GetPlayerTeam(playerId);
        }

        public async Task<List<int>> GetTightEnds()
        {
            return await _playerRepository.GetTightEnds();
        }

        public async Task<int> GetPlayerId(string name)
        {
            return await _playerRepository.GetPlayerId(name);
        }

        #region Private Methods
        private async Task<Player> GetPlayerInfo(int playerId)
        {
            return await _playerRepository.GetPlayerInfo(playerId);
        }
        private async Task<List<PassingStatisticWithSeason>> GetPassingStatisticsWithSeason(int playerId)
        {
            return await _playerRepository.GetPassingStatisticsWithSeason(playerId);
        }
        private async Task<List<RushingStatisticWithSeason>> GetRushingStatisticsWithSeason(int playerId)
        {
            return await _playerRepository.GetRushingStatisticsWithSeason(playerId);
        }
        private async Task<List<ReceivingStatisticWithSeason>> GetReceivingStatisticsWithSeason(int playerId)
        {
            return await _playerRepository.GetReceivingStatisticsWithSeason(playerId);
        }


        #endregion
    }
}
