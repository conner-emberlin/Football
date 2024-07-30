using Microsoft.AspNetCore.Mvc;
using Football.Models;
using Football.Enums;
using Football.Api.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;
using AutoMapper;
using Football.Fantasy.Interfaces;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController(IPlayersService playersService, IStatisticsService statisticsService, IOptionsMonitor<Season> season, IAdvancedStatisticsService advancedStatisticsService, IMapper mapper, IFantasyDataService fantasyDataService) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpGet("data/players")]
        [ProducesResponseType(typeof(List<Player>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPlayers([FromQuery] int active, [FromQuery] string position = "") 
        {
            var allPlayers = await playersService.GetAllPlayers();
            var filteredPlayers =  
                                  active == 1 && position != string.Empty ? allPlayers.Where(p => p.Active == 1 && p.Position == position)
                                : active == 1 && position == string.Empty ? allPlayers.Where(p => p.Active == 1)
                                : position != string.Empty && active != 1 ? allPlayers.Where(p => p.Position == position)
                                : allPlayers;

            return Ok(filteredPlayers.OrderBy(p => p.Name));
        }

        [HttpGet("{playerId}")]
        [ProducesResponseType(typeof(PlayerDataModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlayer(int playerId)
        {
            
            var player = await playersService.GetPlayer(playerId);
            if (player == null) return NotFound();
            var playerModel = mapper.Map<PlayerDataModel>(player);
            _ = Enum.TryParse(player.Position, out Position position);

            var playerTeam = await playersService.GetPlayerTeam(_season.CurrentSeason, playerId);
            if (playerTeam != null) 
            {
                playerModel.Team = playerTeam.Team;
                playerModel.Schedule = mapper.Map<List<ScheduleModel>>(await playersService.GetUpcomingGames(playerId));

            }

            playerModel.WeeklyData = position switch
            {
                Position.QB => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataQB>(position, playerId)),
                Position.WR => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataWR>(position, playerId)),
                Position.RB => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataRB>(position, playerId)),
                Position.TE => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataTE>(position, playerId)),
                Position.DST => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataDST>(position, playerId)),
                Position.K => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataK>(position, playerId)),
                _ => throw new NotImplementedException()
            };

            playerModel.SeasonData = position switch
            {
                Position.QB => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataQB>(position, playerId, true)),
                Position.WR => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataWR>(position, playerId, true)),
                Position.RB => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataRB>(position, playerId, true)),
                Position.TE => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataTE>(position, playerId, true)),
                Position.DST => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataDST>(position, playerId, true)),
                _ => throw new NotImplementedException()
            };

            playerModel.SeasonFantasy = mapper.Map<List<SeasonFantasyModel>>(await fantasyDataService.GetSeasonFantasy(playerId));
            playerModel.WeeklyFantasy = mapper.Map<List<WeeklyFantasyModel>>(await fantasyDataService.GetWeeklyFantasy(playerId));

            if (playerModel.WeeklyFantasy.Count > 0) 
            {
                var currentTotals = await fantasyDataService.GetCurrentFantasyTotals(playerId);
                playerModel.RunningFantasyTotal = currentTotals.First(c => c.PlayerId == playerId).FantasyPoints;
                playerModel.OverallRank = currentTotals.Select(p => p.PlayerId).ToList().IndexOf(playerId);
                playerModel.PositionRank = currentTotals.Where(c => c.Position == player.Position).Select(p => p.PlayerId).ToList().IndexOf(playerId);
            }
           
            return Ok(playerModel);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePlayer([FromBody] Player player) => Ok(await playersService.CreatePlayer(player.Name, player.Active, player.Position));

        [HttpGet("weekly-data/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyData(int playerId)
        {
            var player = await playersService.GetPlayer(playerId);
            if (player == null) return NotFound();
            if (!Enum.TryParse(player.Position, out Position position)) return BadRequest();
            List<WeeklyDataModel> models = [];

            models = position switch
            {
                Position.QB => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataQB>(position, playerId)),
                Position.WR => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataWR>(position, playerId)),
                Position.RB => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataRB>(position, playerId)),
                Position.TE => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataTE>(position, playerId)),
                Position.DST => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataDST>(position, playerId)),
                Position.K => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyData<WeeklyDataK>(position, playerId)),
                _ => models
            };

            return Ok(models);
        }

        [HttpGet("season-data/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonData(int playerId)
        {
            var player = await playersService.GetPlayer(playerId);
            if (player == null) return NotFound();
            if (!Enum.TryParse(player.Position, out Position position)) return BadRequest();
            List<SeasonDataModel> models = [];

            models = position switch
            {
                Position.QB => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataQB>(position, playerId, true)),
                Position.WR => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataWR>(position, playerId, true)),
                Position.RB => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataRB>(position, playerId, true)),
                Position.TE => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataTE>(position, playerId, true)),
                Position.DST => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataDST>(position, playerId, true)),
                _ => models
            };

            return Ok(models);
        }


        [HttpGet("team/{playerId}")]
        [ProducesResponseType(typeof(PlayerTeam), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPlayerTeam(int playerId) => playerId > 0 ? Ok(await playersService.GetPlayerTeam(_season.CurrentSeason, playerId)) : BadRequest();

        [HttpGet("schedule/{playerId}")]
        [ProducesResponseType(typeof(List<Schedule>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUpcomingGames(int playerId) => playerId > 0 ? Ok(await playersService.GetUpcomingGames(playerId)) : BadRequest();

        [HttpGet("schedule/weekly")]
        [ProducesResponseType(typeof(List<Schedule>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGames() => Ok(await playersService.GetGames(_season.CurrentSeason, await playersService.GetCurrentWeek(_season.CurrentSeason)));

        [HttpGet("current-week")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentWeek() => Ok(await playersService.GetCurrentWeek(_season.CurrentSeason));

        [HttpPost("injury")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostInSeasonInjury([FromBody] InSeasonInjury injury) => Ok(await playersService.PostInSeasonInjury(injury));

        [HttpGet("player-injuries")]
        [ProducesResponseType(typeof(List<PlayerInjury>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPlayerInjuries() => Ok(await playersService.GetPlayerInjuries());

        [HttpPut("update-injury")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateInjury([FromBody] InSeasonInjury injury) => Ok(await playersService.UpdateInjury(injury));

        [HttpPost("team-change/in-season")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostInSeasonTeamChange([FromBody] InSeasonTeamChange teamChange) => Ok(await playersService.PostInSeasonTeamChange(teamChange));

        [HttpGet("advanced-stats/qb")]
        [ProducesResponseType(typeof(List<AdvancedQBStatistics>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAdvancedQBStatistics() => Ok(await advancedStatisticsService.GetAdvancedQBStatistics());

        [HttpGet("snap-counts/{playerId}")]
        [ProducesResponseType(typeof(List<SnapCount>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSnapCounts([FromRoute] int playerId) => Ok(await statisticsService.GetSnapCounts(playerId));

        [HttpPost("inactivate")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> InactivatePlayers([FromBody] List<int> playerIds) => Ok(await playersService.InactivatePlayers(playerIds));

        [HttpPost("add-rookie")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateRookie([FromBody] RookiePlayerModel model)
        {
            if (model.Name != string.Empty && Enum.TryParse(model.Position, out Position _))
            {                
                var rookie = mapper.Map<Rookie>(model);

                var teamId = await playersService.GetTeamId(rookie.TeamDrafted);
                if (teamId == 0) return NotFound();
                rookie.TeamId = teamId;

                var existingPlayerId = await playersService.GetPlayerId(model.Name);
                if (existingPlayerId == 0)
                {
                    _= await playersService.CreatePlayer(model.Name, model.Active, model.Position);
                    var playerId = await playersService.GetPlayerId(model.Name);
                    if(playerId > 0)
                    {                       
                        rookie.PlayerId = playerId;
                        return Ok(await playersService.CreateRookie(rookie));
                    }
                        
                }
                else
                {
                    rookie.PlayerId = existingPlayerId;
                    return Ok(await playersService.CreateRookie(rookie));
                }

            }
            return BadRequest();
        }

        [HttpGet("rookies/all")]
        [ProducesResponseType(typeof(List<RookiePlayerModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRookies()
        {
            List<RookiePlayerModel> models = [];
            var rookies = await playersService.GetAllRookies();
            foreach (var rookie in rookies)
            {
                var player = await playersService.GetPlayer(rookie.PlayerId);
                var rmp = mapper.Map<RookiePlayerModel>(rookie);
                rmp.Name = player.Name;
                rmp.Active = player.Active;
                models.Add(rmp);
            }
            return Ok(models.OrderByDescending(m => m.RookieSeason).ThenBy(m => m.Position).ThenBy(m => m.Name).ToList());
        }

        [HttpGet("trending-players")]
        [ProducesResponseType(typeof(List<TrendingPlayerModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTrendingPlayers() => Ok(mapper.Map<List<TrendingPlayerModel>>(await playersService.GetTrendingPlayers()));

        [HttpPost("sleeper-map")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> UploadSleeperPlayerMap() => Ok(await playersService.UploadSleeperPlayerMap());

    }
}
