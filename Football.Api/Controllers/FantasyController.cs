﻿using Football.Models;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Football.Api.Models;
using Football.Players.Interfaces;
using AutoMapper;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController(IFantasyDataService fantasyDataService, IMatchupAnalysisService matchupAnalysisService, IMarketShareService marketShareService,
        IOptionsMonitor<Season> season, IStartOrSitService startOrSitService, IWaiverWireService waiverWireService,
        IPlayersService playersService, IFantasyAnalysisService fantasyAnalysisService, ISnapCountService snapCountService, IMapper mapper) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpPost("data/{position}/{season}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSeasonFantasy(int season, string position)
        {
            if (season > 0 && Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
                return Ok(await fantasyDataService.PostSeasonFantasy(season, positionEnum));
            return BadRequest();
        }
        [HttpPost("data/{position}/{season}/{week}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostWeeklyFantasy(int season, int week, string position)
        {
            if (season > 0 && week > 0 && Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
                return Ok(await fantasyDataService.PostWeeklyFantasy(season, week, positionEnum));
            return BadRequest();
        }

        [HttpGet("season-fantasy/{playerId}")]
        [ProducesResponseType(typeof(List<SeasonFantasyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonFantasy(int playerId) => playerId > 0 ? Ok(mapper.Map<List<SeasonFantasyModel>>(await fantasyDataService.GetSeasonFantasy(playerId))) : BadRequest();

        [HttpGet("data/weekly/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyFantasy(int playerId) => playerId > 0 ? Ok(await fantasyDataService.GetWeeklyFantasy(playerId)) : BadRequest();

        [HttpGet("data/weekly/leaders/{week}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyFantasyLeaders(int week) => week > 0 ? Ok(await fantasyDataService.GetWeeklyFantasy(_season.CurrentSeason, week)) : BadRequest();
                                
        [HttpGet("season-totals")]
        [ProducesResponseType(typeof(List<SeasonFantasy>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentFantasyTotals() => Ok(await fantasyDataService.GetCurrentFantasyTotals(_season.CurrentSeason));

        [HttpGet("matchup-rankings/{position}")]
        [ProducesResponseType(typeof(List<MatchupRankingModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMatchupRankings(string position)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
                var model = mapper.Map<List<MatchupRankingModel>>(await matchupAnalysisService.GetPositionalMatchupRankingsFromSQL(positionEnum, _season.CurrentSeason, currentWeek));
                var teamDictionary = (await playersService.GetAllTeams()).ToDictionary(t => t.TeamId, t => t.TeamDescription);
                model.ForEach(m => m.TeamDescription = teamDictionary[m.TeamId]);
                return Ok(model);
            }  
            return BadRequest();
        }

        [HttpGet("fantasy-analysis/{pos}")]
        [ProducesResponseType(typeof(List<FantasyAnalysisModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFantasyAnalysis([FromRoute] string pos)
        {
            if (Enum.TryParse(pos.Trim().ToUpper(), out Position position))
            {
                var models = mapper.Map<List<FantasyAnalysisModel>>(await fantasyAnalysisService.GetFantasyPerformances(position));
                var boomBustDictionary = (await fantasyAnalysisService.GetBoomBusts(position)).ToDictionary(b => b.Player.PlayerId);
                foreach (var m in models)
                {
                    m.BoomPercentage = boomBustDictionary[m.PlayerId].BoomPercentage;
                    m.BustPercentage = boomBustDictionary[m.PlayerId].BustPercentage;
                }
                return Ok(models);
            }
            return BadRequest();
        }

        [HttpGet("top-opponents/{teamId}/{position}")]
        [ProducesResponseType(typeof(List<WeeklyFantasy>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTopOpponents(int teamId, string position) => Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum) ? Ok(await matchupAnalysisService.GetTopOpponents(positionEnum, teamId)) : BadRequest();

        [HttpGet("team-totals")]
        [ProducesResponseType(typeof(List<TeamTotals>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeamTotals() => Ok(await marketShareService.GetTeamTotals());

        [HttpGet("marketshare/{position}")]
        [ProducesResponseType(typeof(List<TeamTotals>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMarketShare(string position) => Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum) ? Ok(await marketShareService.GetMarketShare(positionEnum)) : BadRequest();


        [HttpGet("targetshares")]
        [ProducesResponseType(typeof(TargetShare), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTargetShares() =>  Ok(await marketShareService.GetTargetShares());

        [HttpPost("start-or-sit")]
        [ProducesResponseType(typeof(List<StartOrSit>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStartOrSit([FromBody] List<int> playerIds) => Ok(await startOrSitService.GetStartOrSits(playerIds));

        [HttpGet("waiver-wire")]
        [ProducesResponseType(typeof(List<WaiverWireCandidate>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWaiverWireCandidates() 
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            return Ok(await waiverWireService.GetWaiverWireCandidates(_season.CurrentSeason, currentWeek));
        }

        [HttpGet("weekly-boom-busts/{playerId}")]
        [ProducesResponseType(typeof(List<BoomBustByWeek>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBoomBustByWeek([FromRoute] int playerId) => playerId > 0 ? Ok(await fantasyAnalysisService.GetBoomBustsByWeek(playerId)) : BadRequest();

        [HttpGet("shares/{position}")]
        [ProducesResponseType(typeof(List<FantasyPerformance>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFantasyPercentages([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await fantasyAnalysisService.GetFantasyPercentages(posEnum)) : BadRequest();

        [HttpGet("snap-analysis/{position}")]
        [ProducesResponseType(typeof(List<SnapCountAnalysis>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSnapCountAnalysis([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await snapCountService.GetSnapCountAnalysis(posEnum, _season.CurrentSeason)) : BadRequest();

    }
}
