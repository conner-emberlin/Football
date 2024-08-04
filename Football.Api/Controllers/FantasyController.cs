using Football.Api.Models.Fantasy;
using Football.Models;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Players.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


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
        [ProducesResponseType(typeof(List<WeeklyFantasyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyFantasy(int playerId) => playerId > 0 ? Ok(mapper.Map<List<WeeklyFantasyModel>>(await fantasyDataService.GetWeeklyFantasy(playerId))) : BadRequest();

        [HttpGet("data/weekly/leaders/{week}")]
        [ProducesResponseType(typeof(List<WeeklyFantasyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyLeaders([FromRoute] int week, [FromQuery] int season) 
        {
            if (week <= 0) return BadRequest();
            
            var fantasySeason = season > 0 ? season : await playersService.GetCurrentWeek(_season.CurrentSeason) == 1 ? _season.CurrentSeason - 1 : _season.CurrentSeason;
            return Ok(mapper.Map<List<WeeklyFantasyModel>>(await fantasyDataService.GetWeeklyFantasy(fantasySeason, week)));
        }

        [HttpGet("season-totals")]
        [ProducesResponseType(typeof(List<SeasonFantasyModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFantasyTotals([FromQuery] int fantasySeason) 
        {
            if (fantasySeason > 0 && fantasySeason != _season.CurrentSeason) return Ok(mapper.Map<List<SeasonFantasyModel>>(await fantasyDataService.GetSeasonFantasy(fantasySeason, false)));
            else
            {
                var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
                if (currentWeek == 1) return Ok(mapper.Map<List<SeasonFantasyModel>>(await fantasyDataService.GetCurrentFantasyTotals(_season.CurrentSeason - 1)));
                return Ok(mapper.Map<List<SeasonFantasyModel>>(await fantasyDataService.GetCurrentFantasyTotals(_season.CurrentSeason)));
            }
            
        } 

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
        [ProducesResponseType(typeof(List<TopOpponentsModel>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTopOpponents(int teamId, string position) 
        {
            if (!Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum)) return BadRequest();

            var teamMap = await playersService.GetTeam(teamId);
            if (teamMap == null) return NotFound();

            var models = mapper.Map<List<TopOpponentsModel>>(await matchupAnalysisService.GetTopOpponents(positionEnum, teamId));
            var playerTeamDictionary = (await playersService.GetPlayerTeams(_season.CurrentSeason, models.Select(m => m.PlayerId))).ToDictionary(p => p.PlayerId, p => p.Team);
            foreach (var m in models)
            {
                m.TopOpponentTeamDescription = teamMap.TeamDescription;
                m.Team = playerTeamDictionary.TryGetValue(m.PlayerId, out var team) ? team : string.Empty;
                var allWeeklyFantasy = await fantasyDataService.GetWeeklyFantasy(m.PlayerId);
                m.AverageFantasy = allWeeklyFantasy.Average(a => a.FantasyPoints);
            }
            return Ok(models);

        } 

        [HttpGet("team-totals")]
        [ProducesResponseType(typeof(List<TeamTotals>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeamTotals() => Ok(await marketShareService.GetTeamTotals());

        [HttpGet("marketshare/{position}")]
        [ProducesResponseType(typeof(List<MarketShareModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMarketShare(string position) => Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum) ? Ok(mapper.Map<List<MarketShareModel>?>(await marketShareService.GetMarketShare(positionEnum))) : BadRequest();


        [HttpGet("targetshares")]
        [ProducesResponseType(typeof(List<TargetShareModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTargetShares() => Ok(mapper.Map<List<TargetShareModel>>(await marketShareService.GetTargetShares()));

        [HttpPost("start-or-sit")]
        [ProducesResponseType(typeof(List<StartOrSitModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStartOrSit([FromBody] List<int> playerIds)
        {
            List<StartOrSitModel> sosModels = [];
            var startOrSits = await startOrSitService.GetStartOrSits(playerIds);
            var teamDictionary = (await playersService.GetAllTeams()).ToDictionary(t => t.TeamId, t => t.Team);
            foreach (var sos in startOrSits)
            {
                var sosModel = mapper.Map<StartOrSitModel>(sos);
                if (sos.ScheduleDetails != null)
                {
                    sosModel.OpponentTeamId = sos.ScheduleDetails.AwayTeamId == sosModel.TeamId ? sos.ScheduleDetails.HomeTeamId : sos.ScheduleDetails.AwayTeamId;
                    sosModel.OpponentTeam = sosModel.OpponentTeamId > 0 ? teamDictionary[sosModel.OpponentTeamId] : string.Empty;
                    sosModel.AtIndicator = sos.ScheduleDetails.HomeTeamId == sosModel.TeamId ? "vs" : "@";
                }
                sosModel.PlayerComparisons = mapper.Map<List<PlayerComparisonModel>>(sos.PlayerComparisons);
                sosModels.Add(sosModel);
            }
            return Ok(sosModels);
        }

        [HttpGet("waiver-wire")]
        [ProducesResponseType(typeof(List<WaiverWireCandidateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWaiverWireCandidates() 
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            return Ok(mapper.Map<List<WaiverWireCandidateModel>>(await waiverWireService.GetWaiverWireCandidates(_season.CurrentSeason, currentWeek)));
        }

        [HttpGet("weekly-boom-busts/{playerId}")]
        [ProducesResponseType(typeof(List<BoomBustByWeek>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBoomBustByWeek([FromRoute] int playerId) => playerId > 0 ? Ok(await fantasyAnalysisService.GetBoomBustsByWeek(playerId)) : BadRequest();

        [HttpGet("shares/{position}")]
        [ProducesResponseType(typeof(List<FantasyPercentageModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFantasyPercentages([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(mapper.Map<List<FantasyPercentageModel>>(await fantasyAnalysisService.GetFantasyPercentages(posEnum))) : BadRequest();

        [HttpGet("snap-analysis/{position}")]
        [ProducesResponseType(typeof(List<SnapCountAnalysisModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSnapCountAnalysis([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(mapper.Map<List<SnapCountAnalysisModel>>(await snapCountService.GetSnapCountAnalysis(posEnum, _season.CurrentSeason))) : BadRequest();

    }
}
