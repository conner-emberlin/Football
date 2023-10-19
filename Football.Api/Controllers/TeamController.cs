﻿using Football.Players.Interfaces;
using Football.Models;
using Football.Players.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IPlayersService _playersService;
        private readonly Season _season;
        public TeamController(IPlayersService playersService, IOptionsMonitor<Season> season)
        {
            _playersService = playersService;
            _season = season.CurrentValue;
        }

        [HttpGet("teams")]
        [ProducesResponseType(typeof(List<TeamMap>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Player>>> GetAllTeams() => Ok(await _playersService.GetAllTeams());

        [HttpGet("players/{team}")]
        [ProducesResponseType(typeof(List<PlayerTeam>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<Player>>> GetPlayersByTeam(string team) => Ok(await _playersService.GetPlayersByTeam(team));

        [HttpGet("location/{teamId}")]
        [ProducesResponseType(typeof(TeamLocation), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<TeamLocation>> GetTeamLocation(int teamId) => Ok(await _playersService.GetTeamLocation(teamId));

        [HttpGet("schedule-details/{week}")]
        [ProducesResponseType(typeof(List<ScheduleDetails>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<List<ScheduleDetails>>> GetScheduleDetails(int week) => Ok(await _playersService.GetScheduleDetails(_season.CurrentSeason, week));
    }
}