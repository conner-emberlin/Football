using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Football.Api.Models;
using AutoMapper;
using Football.Players.Models;


namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectionController(IPlayersService playersService,
        IProjectionAnalysisService analysisService, IOptionsMonitor<Season> season,
        IProjectionService<WeekProjection> weekProjectionService, IProjectionService<SeasonProjection> seasonProjectionService, IMapper mapper) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpGet("season/{position}")]
        [ProducesResponseType(typeof(List<SeasonProjectionModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonProjections(string position)
        {
            if (!Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum)) return BadRequest();

            List<SeasonProjectionModel> model = [];

            if (positionEnum == Position.FLEX)
                model = mapper.Map<List<SeasonProjectionModel>>(analysisService.SeasonFlexRankings());

            else if (seasonProjectionService.GetProjectionsFromCache(positionEnum, out var projectionsCache))
                model = mapper.Map<List<SeasonProjectionModel>>(projectionsCache);

            else if (seasonProjectionService.GetProjectionsFromSQL(positionEnum, _season.CurrentSeason, out var projectionsSQL))
            {
                model = mapper.Map<List<SeasonProjectionModel>>(projectionsSQL);
                model.ForEach(m => m.CanDelete = true);
            }
            else
                model = mapper.Map<List<SeasonProjectionModel>>(await seasonProjectionService.GetProjections(positionEnum));

            var teamDictionary = (await playersService.GetPlayerTeams(_season.CurrentSeason, model.Select(m => m.PlayerId))).ToDictionary(p => p.PlayerId, p => p.Team);
            model.ForEach(m => m.Team = teamDictionary.TryGetValue(m.PlayerId, out var team) ? team : string.Empty);
            var retVal = positionEnum == Position.FLEX ? model : model.OrderByDescending(m => m.ProjectedPoints).ToList();
            return Ok(retVal);
        }
        [HttpGet("season/player/{playerId}")]
        [ProducesResponseType(typeof(SeasonProjection), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonProjections(int playerId) => playerId > 0 ? Ok((await seasonProjectionService.GetPlayerProjections(playerId)).Where(p => p.Season == _season.CurrentSeason).First()) : BadRequest();

        [HttpPost("season/{position}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSeasonProjections(string position)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))  
            {
                var proj = (await seasonProjectionService.GetProjections(positionEnum)).ToList();
                return Ok(await seasonProjectionService.PostProjections(proj));
            }
            return BadRequest();
        }

        [HttpGet("weekly/{position}")]
        [ProducesResponseType(typeof(IEnumerable<WeekProjectionModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyProjections(string position)
        {
            if (!Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum)) return BadRequest();

            List<WeekProjectionModel> models = [];
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            if (currentWeek > _season.Weeks)
                return Ok(models);

            if (positionEnum == Position.FLEX)
                models = mapper.Map<List<WeekProjectionModel>>(await analysisService.WeeklyFlexRankings());

            else if (weekProjectionService.GetProjectionsFromCache(positionEnum, out var projectionsCache))
                models = mapper.Map<List<WeekProjectionModel>>(projectionsCache);

            else if (weekProjectionService.GetProjectionsFromSQL(positionEnum, currentWeek, out var projectionsSQL))
            {
                models = mapper.Map<List<WeekProjectionModel>>(projectionsSQL);
                models.ForEach(m => m.CanDelete = true);
            }
            else
                models = mapper.Map<List<WeekProjectionModel>>(await weekProjectionService.GetProjections(positionEnum));

            var teamDictionary = (await playersService.GetPlayerTeams(_season.CurrentSeason, models.Select(m => m.PlayerId))).ToDictionary(p => p.PlayerId);
            var scheduleDictionary = (await playersService.GetWeeklySchedule(_season.CurrentSeason, currentWeek)).ToDictionary(s => s.TeamId);
            models.ForEach(m => m.Team = teamDictionary.TryGetValue(m.PlayerId, out var team) ? team.Team : string.Empty);
            models.ForEach(m => m.Opponent = teamDictionary.TryGetValue(m.PlayerId, out var team) ? scheduleDictionary[team.TeamId].OpposingTeam : string.Empty);

            return Ok(models.OrderByDescending(m => m.ProjectedPoints));

        }
        [HttpPost("weekly/{position}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostWeeklyProjections(string position)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                var proj = (await weekProjectionService.GetProjections(positionEnum)).ToList();
                return Ok(await weekProjectionService.PostProjections(proj));
            }
            return BadRequest();
        }

        [HttpGet("weekly-analysis/{position}")]
        [ProducesResponseType(typeof(List<WeeklyProjectionAnalysis>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyProjectionAnalysis(string position)
        {
            if (Enum.TryParse(position, out Position positionEnum))
            {
                List<WeeklyProjectionAnalysis> projectionAnalyses = [];
                var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
                for (int i = 1; i < currentWeek; i++)
                {
                    projectionAnalyses.Add(await analysisService.GetWeeklyProjectionAnalysis(positionEnum, i));
                }
                return Ok(projectionAnalyses);
            }
            return BadRequest();
        }

        [HttpGet("weekly-analysis/player/{playerId}")]
        [ProducesResponseType(typeof(WeeklyProjectionAnalysis), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyProjectionAnalysis(int playerId) => playerId > 0 ? Ok(await analysisService.GetWeeklyProjectionAnalysis(playerId)) : BadRequest();

        [HttpGet("weekly-error/{position}/{week}")]
        [ProducesResponseType(typeof(List<WeeklyProjectionError>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyProjectionError(string position, int week) => Enum.TryParse(position, out Position positionEnum) ? Ok(await analysisService.GetWeeklyProjectionError(positionEnum, week)) : BadRequest();

        [HttpGet("weekly-error/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyProjectionError>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyProjectionError(int playerId) => playerId > 0 ? Ok(await analysisService.GetWeeklyProjectionError(playerId)) : BadRequest();

        [HttpGet("sleeper-projections/{username}")]
        [ProducesResponseType(typeof(List<WeekProjection>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSleeperLeagueProjections([FromRoute] string username) => Ok(await analysisService.GetSleeperLeagueProjections(username));

        [HttpGet("sleeper-projections/{username}/matchup")]
        [ProducesResponseType(typeof(List<MatchupProjections>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMatchupProjections([FromRoute] string username)
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            return Ok(await analysisService.GetMatchupProjections(username, currentWeek));
        }

        [HttpDelete("weekly/{playerId}/{season}/{week}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteWeeklyProjection(int playerId, int season, int week)
        {
            var projection = (await weekProjectionService.GetPlayerProjections(playerId)).FirstOrDefault(p => p.Week == week && p.Season == season);
            return projection != null ? Ok(await weekProjectionService.DeleteProjection(projection)) : BadRequest();
        }

        [HttpDelete("season/{playerId}/{season}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSeasonProjection(int playerId, int season)
        {
            var projection = (await seasonProjectionService.GetPlayerProjections(playerId)).FirstOrDefault(p => p.Season == season);
            return projection != null ? Ok(await seasonProjectionService.DeleteProjection(projection)) : BadRequest();
        }

        [HttpGet("season-projection-error/{position}")]
        [ProducesResponseType(typeof(List<SeasonProjectionError>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonProjectionError([FromRoute] string position, [FromQuery] int season) => Enum.TryParse(position, out Position posEnum) ? Ok(await analysisService.GetSeasonProjectionError(posEnum, season)) : BadRequest();

        [HttpGet("season-projection-analysis/{position}")]
        [ProducesResponseType(typeof(SeasonProjectionAnalysis), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonProjectionAnalysis([FromRoute] string position, [FromQuery] int season) => Enum.TryParse(position, out Position posEnum) ? Ok(await analysisService.GetSeasonProjectionAnalysis(posEnum, season)) : BadRequest();

        [HttpGet("season-projection-analysis/all/{position}")]
        [ProducesResponseType(typeof(List<SeasonProjectionAnalysis>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllSeasonProjectionAnalyses([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(await analysisService.GetAllSeasonProjectionAnalyses(posEnum)) : BadRequest();

    }
}
