﻿using Football.Api.Helpers;
using Football.Models;
using Football.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController : ControllerBase
    {
        //POST fantasy results for a season/position (delete existing ones for the season/position)
        [HttpPost("refresh/{playerId}/{season}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<(int,int)> RefreshFantasyPoints(int playerId, int season)
        { 
            FantasyService fantasyService = new();
            var fantasyPoints = fantasyService.GetFantasyPoints(playerId, season);
            return Ok(fantasyService.RefreshFantasyResults(fantasyPoints));

        }
        //GET fantasy results for playerId, season
        [HttpGet("points/{playerId}/{season}")]
        [ProducesResponseType(typeof(FantasyPoints), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public ActionResult<FantasyPoints> GetFantasyPoints(int playerId, int season)
        {
            FantasyService fantasyService = new();
            return Ok(fantasyService.GetFantasyPoints(playerId, season));
        }

        //POST complete fantasy refresh for a season
        //Use to maintain week to week stats for current season
    }
}
