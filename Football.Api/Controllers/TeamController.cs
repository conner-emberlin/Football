using AutoMapper;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Shared.Models.Fantasy;
using Football.Shared.Models.Players;
using Football.Shared.Models.Teams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController(IPlayersService playersService, IStatisticsService statisticsService, IFantasyAnalysisService fantasyAnalysisService,
        IOptionsMonitor<Season> season, IFantasyDataService fantasyDataService, ITeamsService teamsService, IMatchupAnalysisService matchupAnalysisService, IMapper mapper) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamMapModel>>> GetAllTeams() => Ok(mapper.Map<List<TeamMapModel>>(await teamsService.GetAllTeams()));

        [HttpGet("schedule-details/current")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetailsModel>>> GetScheduleDetails() => Ok(mapper.Map<List<ScheduleDetailsModel>>(await teamsService.GetScheduleDetails(_season.CurrentSeason, await playersService.GetCurrentWeek(_season.CurrentSeason))));

        [HttpGet("team-records")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamRecordModel>>> GetTeamRecords() => Ok(mapper.Map<List<TeamRecordModel>>(await teamsService.GetTeamRecords(_season.CurrentSeason)));

        [HttpGet("game-results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GameResultModel>>> GetGameResults() => Ok(mapper.Map<List<GameResultModel>>(await statisticsService.GetGameResults(_season.CurrentSeason)));

        [HttpGet("fantasy-performances/{teamId}")]
        [ProducesResponseType(typeof(List<FantasyPerformanceModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFantasyPerformances([FromRoute] int teamId) => teamId > 0 ? Ok(mapper.Map<List<FantasyPerformanceModel>>(await fantasyAnalysisService.GetFantasyPerformances(teamId))) : BadRequest();

        [HttpGet("weekly-fantasy/{team}/{week}")]
        [ProducesResponseType(typeof(List<WeeklyFantasyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyTeamFantasy([FromRoute] string team, [FromRoute] int week) => week > 0 ? Ok(mapper.Map<List<WeeklyFantasyModel>>(await fantasyDataService.GetWeeklyTeamFantasy(team, week))) : BadRequest();

        [HttpGet("league-information/{teamId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamLeagueInformationModel>>> GetTeamLeagueInformation([FromRoute] int teamId) => Ok(mapper.Map<TeamLeagueInformationModel>(await teamsService.GetTeamLeagueInformation(teamId)));

        [HttpGet("team-records/division/{teamId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamRecordModel>>> GetTeamRecordInDivision([FromRoute] int teamId) => Ok(mapper.Map<TeamRecordModel>(await teamsService.GetTeamRecordInDivision(teamId)));

        [HttpGet("depth-chart/{teamId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeamDepthChartModel>>> GetTeamDepthChart([FromRoute] int teamId, [FromQuery] bool includeSpecialTeams = false) 
        {
            var depthChart = await teamsService.GetTeamDepthChart(teamId);
            var model =  includeSpecialTeams ? mapper.Map<List<TeamDepthChartModel>>(depthChart)
                                       : mapper.Map<List<TeamDepthChartModel>>(depthChart).Where(d => d.Position != Position.K.ToString() && d.Position != Position.DST.ToString());

            var inSeasonInjuries = (await playersService.GetActiveInSeasonInjuries(_season.CurrentSeason)).ToDictionary(a => a.PlayerId);
            foreach (var m in model)
            {
                if (inSeasonInjuries.TryGetValue(m.PlayerId, out var injury)) m.ActiveInjury = mapper.Map<InSeasonInjuryModel>(injury);
            }
            return Ok(model);
        }

        [HttpGet("ros-matchup-rankings/{teamId}")]
        [ProducesResponseType(typeof(List<MatchupRankingModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRestOfSeasonMatchupRankings([FromRoute] int teamId)
        {
            var allTeamsDictionary = (await teamsService.GetAllTeams()).ToDictionary(t => t.TeamId, t => t.TeamDescription);
            if (!allTeamsDictionary.TryGetValue(teamId, out var teamDescription)) return BadRequest();

            var models = mapper.Map<List<MatchupRankingModel>>(await matchupAnalysisService.GetRestOfSeasonMatchupRankingsByTeam(teamId));
            var matchupRankings = await matchupAnalysisService.GetCurrentMatchupRankings();

            foreach (var model in models)
            {
                model.RequestedTeamDescription = teamDescription;
                model.TeamDescription = allTeamsDictionary[model.TeamId];
                var positionalRankings = matchupRankings.Where(mr => mr.Position == model.Position).OrderBy(m => m.AvgPointsAllowed).ToList();
                model.Ranking = positionalRankings.FindIndex(p => p.TeamId == model.TeamId) + 1;
            }
            return Ok(models);
        }

        [HttpGet("upcoming-games/{teamPlayerId}")]
        [ProducesResponseType(typeof(List<ScheduleModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUpcomingGames(int teamPlayerId)
        {
            return Ok(mapper.Map<List<ScheduleModel>>(await teamsService.GetUpcomingGames(teamPlayerId)));
        }

        [HttpGet("players-without-teams/{season}/{position}")]
        [ProducesResponseType(typeof(List<SimplePlayerModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPlayersWithoutTeams(int season, string position)
        {
            if (!Enum.TryParse(position, out Position posEnum)) return BadRequest();

            return Ok(mapper.Map<List<SimplePlayerModel>>(await teamsService.GetPlayersWithoutTeams(season, posEnum.ToString())));
        }

        [HttpPost("player-teams")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostPlayerTeams([FromBody] List<PlayerTeamModel> playerTeams)
        {
            return Ok(await teamsService.PostPlayerTeams(mapper.Map<List<PlayerTeam>>(playerTeams)));
        }
    }
}
