using Football.Interfaces;
using Football.Models;
using System.Data;
using Dapper;

namespace Football.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ISqlQueryService _sqlQueryService;
        private readonly IDbConnection _dbConnection;

        public PlayerRepository(ISqlQueryService sqlQueryService, IDbConnection dbConnection)
        {
            _sqlQueryService = sqlQueryService;
            _dbConnection = dbConnection;
        }

        public async Task<PassingStatistic> GetPassingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetPassingStatistic();
            return await _dbConnection.QueryFirstOrDefaultAsync<PassingStatistic>(query, new { season, playerId });
        }

        public async Task<PassingStatisticWithSeason> GetPassingStatisticWithSeason(int playerId, int season)
        {
            var query = _sqlQueryService.GetPassingStatisticWithSeason();
            return await _dbConnection.QueryFirstOrDefaultAsync<PassingStatisticWithSeason>(query, new { season, playerId });
        }
        public async Task<List<PassingStatisticWithSeason>> GetPassingStatisticsWithSeason(int playerId)
        {
            var query = _sqlQueryService.GetPassingStatisticsWithSeason();
            var stats = await _dbConnection.QueryAsync<PassingStatisticWithSeason>(query, new { playerId });
            return stats.ToList();             
        }

        public async Task<RushingStatistic> GetRushingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetRushingStatistic();
            return await _dbConnection.QueryFirstOrDefaultAsync<RushingStatistic>(query, new { season, playerId });
        }

        public async Task<RushingStatisticWithSeason> GetRushingStatisticWithSeason(int playerId, int season)
        {
            var query = _sqlQueryService.GetRushingStatisticWithSeason();
            return await _dbConnection.QueryFirstOrDefaultAsync<RushingStatisticWithSeason>(query, new { season, playerId });
        }
        public async Task<List<RushingStatisticWithSeason>> GetRushingStatisticsWithSeason(int playerId)
        {
            var query = _sqlQueryService.GetRushingStatisticsWithSeason();
            var stats =  await _dbConnection.QueryAsync<RushingStatisticWithSeason>(query, new { playerId });
            return stats.ToList();
        }
        public async Task<ReceivingStatistic> GetReceivingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetReceivingStatistic();
            return await _dbConnection.QueryFirstOrDefaultAsync<ReceivingStatistic>(query, new { season, playerId }); ;
        }

        public async Task<ReceivingStatisticWithSeason> GetReceivingStatisticWithSeason(int playerId, int season)
        {
            var query = _sqlQueryService.GetReceivingStatisticWithSeason();
            return await _dbConnection.QueryFirstOrDefaultAsync<ReceivingStatisticWithSeason>(query, new { season, playerId });
        }
        public async Task<List<ReceivingStatisticWithSeason>> GetReceivingStatisticsWithSeason(int playerId)
        {
            var query = _sqlQueryService.GetReceivingStatisticsWithSeason();
            var stats = await _dbConnection.QueryAsync<ReceivingStatisticWithSeason>(query, new { playerId });
            return stats.ToList();
        }
        public async Task<string> GetPlayerPosition(int playerId)
        {
            var query = _sqlQueryService.GetPlayerPosition();
            return await _dbConnection.QueryFirstOrDefaultAsync<string>(query, new { playerId });
        }

        public async Task<List<int>> GetPlayersByPosition(string position)
        {
            var query = _sqlQueryService.GetPlayersByPosition();
            var players = await _dbConnection.QueryAsync<int>(query, new { position });
            return players.ToList();
        }
        public async Task<List<int>> GetPlayerIdsByFantasySeason(int season)
        {
            var query = _sqlQueryService.GetPlayerIdsByFantasySeason();
            var players = await _dbConnection.QueryAsync<int>(query, new { season });
            return players.ToList();
        }

        public async Task<List<int>> GetActiveSeasons(int playerId)
        {
            var query = _sqlQueryService.GetSeasons();
            var seasons = await _dbConnection.QueryAsync<int>(query, new { playerId });
            return seasons.ToList();
        }

        public async Task<List<FantasySeasonGames>> GetFantasySeasonGames(int playerId)
        {
            var query = _sqlQueryService.GetFantasyGames();
            var games = await _dbConnection.QueryAsync<FantasySeasonGames>(query, new { playerId });
            return games.ToList();
        }

        public async Task<List<int>> GetActivePassingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActivePassingSeasons();
            var seasons = await _dbConnection.QueryAsync<int>(query, new { playerId });
            return seasons.ToList();
        }

        public async Task<List<int>> GetActiveRushingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActiveRushingSeasons();
            var seasons = await _dbConnection.QueryAsync<int>(query, new { playerId });
            return seasons.ToList();
        }

        public async Task<List<int>> GetActiveReceivingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActiveReceivingSeasons();
            var seasons = await _dbConnection.QueryAsync<int>(query, new { playerId });
            return seasons.ToList();
        }

        public async Task<string> GetPlayerName(int playerId)
        {
            var query = _sqlQueryService.GetPlayerName();
            var players = await _dbConnection.QueryAsync<string>(query, new { playerId });
            return players.FirstOrDefault().ToString();
        }

        public async Task<bool> IsPlayerActive(int playerId)
        {
            var query = _sqlQueryService.IsPlayerActive();
            var truth = await _dbConnection.QueryAsync<int>(query, new { playerId });
            return truth.FirstOrDefault() == 1;
        }

        public async Task<string> GetPlayerTeam(int playerId)
        {
            var query = _sqlQueryService.GetPlayerTeam();
            var team = await _dbConnection.QueryAsync<string>(query, new { playerId });
            return team.FirstOrDefault().ToString();
        }

        public async Task<List<int>> GetTightEnds()
        {
            var query = _sqlQueryService.GetTightEnds();
            var tightEnds = await _dbConnection.QueryAsync<int>(query);
            return tightEnds.ToList();
        }

        public async Task<Player> GetPlayerInfo(int playerId)
        {
            var query = _sqlQueryService.GetPlayerInfo();
            var player = await _dbConnection.QueryAsync<Player>(query, new { playerId });
            return player.FirstOrDefault();
        }

        public async Task<int> GetPlayerId(string name)
        {
            var query = _sqlQueryService.GetPlayerId();
            var id = await _dbConnection.QueryFirstOrDefaultAsync<int>(query, new { name });
            return id;
        }
        public async Task<int> AddPassingStat(PassingStatisticWithSeason pass, int playerId)
        {
            var query = _sqlQueryService.AddPassingStat();
            int count = 0;
            return count += await _dbConnection.ExecuteAsync(query, new
            {
                pass.Season,
                pass.Name,
                pass.Team,
                pass.Age,
                pass.Games,
                pass.Completions,
                pass.Attempts,
                pass.Yards,
                pass.Touchdowns,
                pass.Interceptions,
                pass.FirstDowns,
                pass.Long,
                pass.Sacks,
                pass.SackYards,
                playerId
            });
        }
        public async Task<int> DeletePassingStats(int playerId)
        {
            var query = _sqlQueryService.DeletePassingStats();
            var count = 0;
            return count += await _dbConnection.ExecuteAsync(query, new
            {
                playerId,               
            });
        }
        public async Task<int> AddRushingStat(RushingStatisticWithSeason rush, int playerId)
        {
            var query = _sqlQueryService.AddRushingStat();
            int count = 0;
            return count += await _dbConnection.ExecuteAsync(query, new
            {
                rush.Season,
                rush.Name,
                rush.Team,
                rush.Age,
                rush.Games,
                rush.RushAttempts,
                rush.Yards,
                rush.Touchdowns,
                rush.FirstDowns,
                rush.Long,
                rush.Fumbles,
                playerId
            });
        }
        public async Task<int> DeleteRushingStats(int playerId)
        {
            var query = _sqlQueryService.DeleteRushingStats();
            var count = 0;
            return count += await _dbConnection.ExecuteAsync(query, new
            {
                playerId,
            });
        }
        public async Task<int> AddReceivingStat(ReceivingStatisticWithSeason rec, int playerId)
        {
            var query = _sqlQueryService.AddReceivingStat();
            int count = 0;
            return count += await _dbConnection.ExecuteAsync(query, new
            {
                rec.Season,
                rec.Name,
                rec.Team,
                rec.Age,
                rec.Games,
                rec.Targets,
                rec.Receptions,
                rec.Yards,
                rec.Touchdowns,
                rec.FirstDowns,
                rec.Long,
                rec.RpG,
                rec.Fumbles,
                playerId
            });
        }
        public async Task<int> DeleteReceivingStats(int playerId)
        {
            var query = _sqlQueryService.DeleteReceivingStats();
            var count = 0;
            return count += await _dbConnection.ExecuteAsync(query, new
            {
                playerId,
            });
        }
        public async Task<int> CreatePlayer(string name, string position, int active)
        {
            var query = _sqlQueryService.CreatePlayer();
            return await _dbConnection.ExecuteAsync(query, new
            {
                name,
                position,
                active
            });
        }
    }
}
