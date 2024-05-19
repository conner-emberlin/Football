﻿using Microsoft.AspNetCore.Mvc;
using Football.Models;
using Football.Enums;
using Football.Api.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;
using AutoMapper;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController(IPlayersService playersService, IStatisticsService statisticsService, IDistanceService distanceService,
        IOptionsMonitor<Season> season, IAdvancedStatisticsService advancedStatisticsService, IMapper mapper) : ControllerBase
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
        [HttpPost("add")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePlayer([FromBody] Player player) => Ok(await playersService.CreatePlayer(player.Name, player.Active, player.Position));


        [HttpGet("data/qb/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataQB>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonDataQB(int playerId) => playerId > 0 ? Ok(await statisticsService.GetSeasonData<SeasonDataQB>(Position.QB, playerId, true)) : BadRequest();

        [HttpGet("data/rb/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataRB>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonDataRB(int playerId) => playerId > 0 ? Ok(await statisticsService.GetSeasonData<SeasonDataRB>(Position.RB, playerId, true)) : BadRequest();

        [HttpGet("data/wr/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataWR>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonDataWR(int playerId) => playerId > 0 ? Ok(await statisticsService.GetSeasonData<SeasonDataWR>(Position.WR, playerId, true)) : BadRequest();

        [HttpGet("data/te/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataTE>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonDataTE(int playerId) => playerId > 0 ? Ok(await statisticsService.GetSeasonData<SeasonDataTE>(Position.TE, playerId, true)) : BadRequest();

        [HttpGet("data/dst/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonDataDST>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonDataDST(int playerId) => playerId > 0 ? Ok(await statisticsService.GetSeasonData<SeasonDataDST>(Position.DST, playerId, true)) : BadRequest();

        [HttpGet("data/weekly/qb/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataQB>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyDataQB(int playerId) => playerId > 0 ? Ok((await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

        [HttpGet("data/weekly/rb/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataRB>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyDataRB(int playerId) => playerId > 0 ? Ok((await statisticsService.GetWeeklyData<WeeklyDataRB>(Position.RB, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

        [HttpGet("data/weekly/wr/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataWR>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyDataWR(int playerId) => playerId > 0 ? Ok((await statisticsService.GetWeeklyData<WeeklyDataWR>(Position.WR, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

        [HttpGet("data/weekly/te/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataTE>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyDataTE(int playerId) => playerId > 0 ? Ok((await statisticsService.GetWeeklyData<WeeklyDataTE>(Position.TE, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

        [HttpGet("data/weekly/dst/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataDST>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyDataDST(int playerId) => playerId > 0 ?  Ok((await statisticsService.GetWeeklyData<WeeklyDataDST>(Position.DST, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

        [HttpGet("data/weekly/k/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyDataK>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyDataK(int playerId) => playerId > 0 ? Ok((await statisticsService.GetWeeklyData<WeeklyDataK>(Position.K, playerId)).Where(w => w.Season == _season.CurrentSeason).ToList()) : BadRequest();

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

        [HttpPost("travel-distance/{playerId}")]
        [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTravelDistance([FromRoute] int playerId) => Ok(await distanceService.GetTravelDistance(playerId));

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
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRookie([FromBody] RookiePlayerModel model)
        {
            if (model.Name != string.Empty && model.Position != string.Empty && Enum.TryParse(model.Position, out Position _))
            {                
                var rookie = mapper.Map<Rookie>(model);
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
        [ProducesResponseType(typeof(List<TrendingPlayer>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTrendingPlayers() => Ok(await playersService.GetTrendingPlayers());

        [HttpPost("sleeper-map")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> UploadSleeperPlayerMap() => Ok(await playersService.UploadSleeperPlayerMap());
    }
}
