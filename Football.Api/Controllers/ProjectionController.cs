using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AutoMapper;
using Football.Shared.Models.Projection;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectionController(IPlayersService playersService, IStatisticsService statisticsService,
        IProjectionAnalysisService analysisService, IOptionsMonitor<Season> season,
        IProjectionService<WeekProjection> weekProjectionService, IProjectionService<SeasonProjection> seasonProjectionService, IMapper mapper) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;


        [HttpPost("season/{position}")]
        [ProducesResponseType(typeof(List<SeasonProjectionModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonProjections(string position, [FromBody] List<string> filter)
        {
            if (!Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum)) return BadRequest();

            List<SeasonProjectionModel> model = [];

            if (positionEnum == Position.FLEX)
                model = mapper.Map<List<SeasonProjectionModel>>(analysisService.SeasonFlexRankings());

            else if (seasonProjectionService.GetProjectionsFromSQL(positionEnum, _season.CurrentSeason, out var projectionsSQL))
            {
                model = mapper.Map<List<SeasonProjectionModel>>(projectionsSQL);
                model.ForEach(m => m.CanDelete = true);
            }
            else
                model = mapper.Map<List<SeasonProjectionModel>>(await seasonProjectionService.GetProjections(positionEnum, filter));

            var teamDictionary = (await playersService.GetPlayerTeams(_season.CurrentSeason, model.Select(m => m.PlayerId))).ToDictionary(p => p.PlayerId, p => p.Team);
            model.ForEach(m => m.Team = teamDictionary.TryGetValue(m.PlayerId, out var team) ? team : string.Empty);

            var currentSeasonGames = await playersService.GetCurrentSeasonGames();
            foreach (var m in model)
            {              
                var avgGamesMissed = await statisticsService.GetAverageGamesMissed(m.PlayerId);                
                m.AvgerageGamesMissed = avgGamesMissed;
                m.AdjustedProjectedPoints = m.ProjectedPoints - (m.ProjectedPoints/currentSeasonGames) * avgGamesMissed;
            }

            var adpDictionary = (await statisticsService.GetAdpByPosition(_season.CurrentSeason, positionEnum)).ToDictionary(a => a.PlayerId);
            foreach (var m in model)
            {
                if(adpDictionary.TryGetValue(m.PlayerId, out var adp))
                {
                    m.PositionalADP = adp.PositionADP;
                    m.OverallADP = adp.OverallADP;
                }
            }

            if (positionEnum == Position.FLEX) return Ok(model);

            else
            {
                var activeSeason = await playersService.GetCurrentWeek(_season.CurrentSeason) <= await playersService.GetCurrentSeasonWeeks();
                if (activeSeason) return Ok(model.OrderByDescending(m => m.ProjectedPoints).ToList());
                else
                {
                    var seasonProjectionErrorDictionary = mapper.Map<List<SeasonProjectionErrorModel>>(await analysisService.GetSeasonProjectionError(positionEnum)).ToDictionary(s => s.PlayerId);
                    model.ForEach(m => m.SeasonProjectionError = seasonProjectionErrorDictionary.TryGetValue(m.PlayerId, out var error) ? error : null);
                    return Ok(model.OrderByDescending(m => m.ProjectedPoints).ToList());
                }
            }
        }

        [HttpPost("season/upload/{position}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSeasonProjections(string position, [FromBody] List<string> filter)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))  
            {
                var proj = (await seasonProjectionService.GetProjections(positionEnum, filter)).ToList();
                return Ok(await seasonProjectionService.PostProjections(proj, filter));
            }
            return BadRequest();
        }

        [HttpGet("season/coefficients/{position}")]
        [ProducesResponseType(typeof(double[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonCoefficients(string position)
        {
            if (!Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum)) return BadRequest();

            return Ok((await seasonProjectionService.GetCoefficients(positionEnum)).ToArray<double>());
        }

        [HttpGet("season/projections-exist/{position}")]
        [ProducesResponseType(typeof(SeasonProjectionsExistModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonProjectionsExist(string position)
        {
            if (!Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum)) return BadRequest();

            var projectionsExist = seasonProjectionService.GetProjectionsFromSQL(positionEnum, _season.CurrentSeason, out var projections);

            if (projectionsExist)
            {
                var filters = (await seasonProjectionService.GetCurrentProjectionConfigurationFilter(positionEnum))?.Split(',').ToList();
                return Ok(new SeasonProjectionsExistModel
                {
                    ProjectionExist = projectionsExist,
                    Filters = filters ?? []
                }) ;
            }

            return Ok(new SeasonProjectionsExistModel { ProjectionExist = false });
        }

        [HttpGet("weekly/coefficients/{position}")]
        [ProducesResponseType(typeof(double[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyCoefficients(string position)
        {
            if (!Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum)) return BadRequest();

            return Ok((await weekProjectionService.GetCoefficients(positionEnum)).ToArray<double>());
        }

        [HttpGet("weekly/{position}")]
        [ProducesResponseType(typeof(IEnumerable<WeekProjectionModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyProjections(string position)
        {
            if (!Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum)) return BadRequest();

            List<WeekProjectionModel> models = [];
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            if (currentWeek > await playersService.GetCurrentSeasonWeeks())
                return Ok(models);

            if (positionEnum == Position.FLEX)
                models = mapper.Map<List<WeekProjectionModel>>(await analysisService.WeeklyFlexRankings());

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
        [HttpPost("weekly/upload/{position}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostWeeklyProjections(string position, [FromBody] List<string> filters)
        {
            if (Enum.TryParse(position.Trim().ToUpper(), out Position positionEnum))
            {
                var proj = (await weekProjectionService.GetProjections(positionEnum, filters)).ToList();
                return Ok(await weekProjectionService.PostProjections(proj, filters));
            }
            return BadRequest();
        }


        [HttpGet("weekly-analysis/{position}")]
        [ProducesResponseType(typeof(List<WeeklyProjectionAnalysisModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyProjectionAnalysis(string position)
        {
            if (Enum.TryParse(position, out Position positionEnum))
            {
                List<WeeklyProjectionAnalysisModel> projectionAnalyses = [];
                var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
                for (int i = 1; i < currentWeek; i++)
                {
                    projectionAnalyses.Add(mapper.Map<WeeklyProjectionAnalysisModel>(await analysisService.GetWeeklyProjectionAnalysis(positionEnum, i)));
                }
                return Ok(projectionAnalyses);
            }
            return BadRequest();
        }

        [HttpGet("weekly-analysis/player/{playerId}")]
        [ProducesResponseType(typeof(WeeklyProjectionAnalysisModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyProjectionAnalysis(int playerId) => Ok(mapper.Map<List<WeeklyProjectionAnalysisModel>>(await analysisService.GetWeeklyProjectionAnalysis(playerId)));

        [HttpGet("weekly-error/{position}/{week}")]
        [ProducesResponseType(typeof(List<WeeklyProjectionErrorModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyProjectionError(string position, int week) => Enum.TryParse(position, out Position positionEnum) ? Ok(mapper.Map<List<WeeklyProjectionErrorModel>>(await analysisService.GetWeeklyProjectionError(positionEnum, week))) : BadRequest();

        [HttpGet("weekly-error/{playerId}")]
        [ProducesResponseType(typeof(List<WeeklyProjectionErrorModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeeklyProjectionError(int playerId) => playerId > 0 ? Ok(mapper.Map<List<WeeklyProjectionErrorModel>>(await analysisService.GetWeeklyProjectionError(playerId))) : BadRequest();

        [HttpGet("sleeper-projections/{username}/matchup")]
        [ProducesResponseType(typeof(List<MatchupProjectionsModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMatchupProjections([FromRoute] string username)
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var matchupProjections = await analysisService.GetMatchupProjections(username, currentWeek);
            var matchupProjectionsModel = mapper.Map<List<MatchupProjectionsModel>>(matchupProjections);
            foreach (var m in matchupProjectionsModel)
            {
                var proj = matchupProjections.First(p => p.TeamName == m.TeamName);
                m.TeamProjections = mapper.Map<List<WeekProjectionModel>>(proj.TeamProjections);
            }
            return Ok(matchupProjectionsModel);
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

        [HttpGet("season-projection-analysis/{position}")]
        [ProducesResponseType(typeof(SeasonProjectionAnalysisModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonProjectionAnalysis([FromRoute] string position, [FromQuery] int season) => Enum.TryParse(position, out Position posEnum) ? Ok(mapper.Map<SeasonProjectionAnalysisModel>(await analysisService.GetSeasonProjectionAnalysis(posEnum, season))) : BadRequest();

        [HttpGet("season-projection-analysis/all/{position}")]
        [ProducesResponseType(typeof(List<SeasonProjectionAnalysisModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllSeasonProjectionAnalyses([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(mapper.Map<List<SeasonProjectionAnalysisModel>>(await analysisService.GetAllSeasonProjectionAnalyses(posEnum))) : BadRequest();

        [HttpGet("season-model-variables/{position}")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetSeasonModelVariablesByPosition([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(seasonProjectionService.GetModelVariablesByPosition(posEnum)) : BadRequest();

        [HttpGet("weekly-model-variables/{position}")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetWeeklyModelVariablesByPosition([FromRoute] string position) => Enum.TryParse(position, out Position posEnum) ? Ok(weekProjectionService.GetModelVariablesByPosition(posEnum)) : BadRequest();


    }
}
