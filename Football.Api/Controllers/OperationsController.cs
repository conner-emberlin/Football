using Football.Enums;
using Football.Models;
using Football.Data.Interfaces;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Shared.Models.Operations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController(IUploadWeeklyDataService weeklyDataService, IFantasyDataService fantasyService, IMatchupAnalysisService matchupAnalysisService, 
        ISettingsService settingsService, IMapper mapper, IOptionsMonitor<Season> season, IPlayersService playersService, IStatisticsService statisticsService, IUploadSeasonDataService seasonDataService) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpPost("weekly-data/{season}/{week}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadWeeklyData(int season, int week)
        {
            if (season > 0 && week > 0)
            {
                var ignoreList = await playersService.GetIgnoreList();
                var count = 0;

                count += await weeklyDataService.UploadWeeklyQBData(season, week, ignoreList);
                count += await weeklyDataService.UploadWeeklyRBData(season, week, ignoreList);
                count += await weeklyDataService.UploadWeeklyWRData(season, week, ignoreList);
                count += await weeklyDataService.UploadWeeklyTEData(season, week, ignoreList);
                count += await weeklyDataService.UploadWeeklyKData(season, week);
                count += await weeklyDataService.UploadWeeklyDSTData(season, week);

                count += await weeklyDataService.UploadWeeklyGameResults(season, week);
               
                count += await weeklyDataService.UploadWeeklyRosterPercentages(season, week, Position.QB.ToString());
                count += await weeklyDataService.UploadWeeklyRosterPercentages(season, week, Position.RB.ToString());
                count += await weeklyDataService.UploadWeeklyRosterPercentages(season, week, Position.WR.ToString());
                count += await weeklyDataService.UploadWeeklyRosterPercentages(season, week, Position.TE.ToString());

                count += await weeklyDataService.UploadWeeklySnapCounts(season, week, Position.QB.ToString());
                count += await weeklyDataService.UploadWeeklySnapCounts(season, week, Position.RB.ToString());
                count += await weeklyDataService.UploadWeeklySnapCounts(season, week, Position.WR.ToString());
                count += await weeklyDataService.UploadWeeklySnapCounts(season, week, Position.TE.ToString());

                count += await weeklyDataService.UploadConsensusWeeklyProjections(week + 1, Position.QB.ToString(), ignoreList);
                count += await weeklyDataService.UploadConsensusWeeklyProjections(week + 1, Position.RB.ToString(), ignoreList);
                count += await weeklyDataService.UploadConsensusWeeklyProjections(week + 1, Position.WR.ToString(), ignoreList);
                count += await weeklyDataService.UploadConsensusWeeklyProjections(week + 1, Position.TE.ToString(), ignoreList);
                count += await weeklyDataService.UploadConsensusWeeklyProjections(week + 1, Position.DST.ToString(), ignoreList);
                count += await weeklyDataService.UploadConsensusWeeklyProjections(week + 1, Position.K.ToString(), ignoreList);
                return Ok(count);
            }
            return BadRequest();
        }

        [HttpPost("weekly-fantasy/{season}/{week}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadWeeklyFantasy(int season, int week)
        {
            if (season > 0 && week > 0)
            {
                var count = 0;
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.QB);
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.RB);
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.WR);
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.TE);
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.DST);
                count += await fantasyService.PostWeeklyFantasy(season, week, Position.K);
                return Ok(count);
            }
            return BadRequest();
        }

        [HttpPost("matchup-rankings")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadMatchupRankings()
        {
            var count = 0;
            var week = await playersService.GetCurrentWeek(_season.CurrentSeason);
            count += await matchupAnalysisService.PostMatchupRankings(Position.QB, week);
            count += await matchupAnalysisService.PostMatchupRankings(Position.RB, week);
            count += await matchupAnalysisService.PostMatchupRankings(Position.WR, week);
            count += await matchupAnalysisService.PostMatchupRankings(Position.TE, week);
            count += await matchupAnalysisService.PostMatchupRankings(Position.DST, week);
            count += await matchupAnalysisService.PostMatchupRankings(Position.K, week);
            return Ok(count);
        }

        [HttpPost("season-tunings")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadSeasonTunings([FromBody] TuningsModel tuningsModel)
        {
            var tunings = mapper.Map<Tunings>(tuningsModel);
            tunings.Season = _season.CurrentSeason;
            return Ok(await settingsService.UploadSeasonTunings(tunings));
        }

        [HttpGet("season-tunings")]
        [ProducesResponseType(typeof(TuningsModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSeasonTunings([FromQuery] int seasonParam)
        {
            var season = seasonParam > 0 ? seasonParam : _season.CurrentSeason;
            var tunings = await settingsService.GetSeasonTunings(season);
            if (tunings != null) 
            { 
                return Ok(mapper.Map<TuningsModel>(tunings));
            }
            else
            {
                var previousSeasonTuningsModel = mapper.Map<TuningsModel>(await settingsService.GetSeasonTunings(season - 1));
                previousSeasonTuningsModel.PreviousSeasonTunings = true;
                return Ok(mapper.Map<TuningsModel>(previousSeasonTuningsModel));
            }                       
        }

        [HttpPost("season-adjustments")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadSeasonAdjustments([FromBody] SeasonAdjustmentsModel seasonAdjustmentsModel)
        {
            var seasonAdjustments = mapper.Map<SeasonAdjustments>(seasonAdjustmentsModel);
            seasonAdjustments.Season = _season.CurrentSeason;
            return Ok(await settingsService.UploadSeasonAdjustments(seasonAdjustments));
        }

        [HttpGet("season-adjustments")]
        [ProducesResponseType(typeof(TuningsModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSeasonAdjustments([FromQuery] int seasonParam)
        {
            var season = seasonParam > 0 ? seasonParam : _season.CurrentSeason;
            return Ok(mapper.Map<SeasonAdjustmentsModel>(await settingsService.GetSeasonAdjustments(season)));
        }

        [HttpPost("weekly-tunings")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadWeeklyTunings([FromBody] WeeklyTuningsModel tuningsModel)
        {
            var tunings = mapper.Map<WeeklyTunings>(tuningsModel);
            tunings.Season = _season.CurrentSeason;
            tunings.Week = await playersService.GetCurrentWeek(_season.CurrentSeason);
            return Ok(await settingsService.UploadWeeklyTunings(tunings));
        }

        [HttpGet("weekly-tunings")]
        [ProducesResponseType(typeof(WeeklyTuningsModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWeeklyTunings([FromQuery] int seasonParam, [FromQuery] int weekParam)
        {
            var season = seasonParam > 0 ? seasonParam : _season.CurrentSeason;
            var week = weekParam > 0 ? weekParam : await playersService.GetCurrentWeek(season);

            return Ok(mapper.Map<WeeklyTuningsModel>(await settingsService.GetWeeklyTunings(season, week)));
        }

        [HttpPut("refresh-adp/{position}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RefreshAdp(string position)
        {
            if (!Enum.TryParse(position.ToUpper(), out Position pos)) return BadRequest();

            _ = await statisticsService.DeleteAdpByPosition(_season.CurrentSeason, pos); 

            if (pos == Position.FLEX)
            {
                var total = await seasonDataService.UploadADP(_season.CurrentSeason, Position.QB.ToString())
                          + await seasonDataService.UploadADP(_season.CurrentSeason, Position.RB.ToString())
                          + await seasonDataService.UploadADP(_season.CurrentSeason, Position.WR.ToString())
                          + await seasonDataService.UploadADP(_season.CurrentSeason, Position.TE.ToString());
                return Ok(total);
            }
            return Ok(await seasonDataService.UploadADP(_season.CurrentSeason, position));
            
        }

        [HttpPut("refresh-consensus-projections/{position}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RefreshConsensusProjections(string position)
        {
            if (!Enum.TryParse(position.ToUpper(), out Position pos)) return BadRequest();

            _ = await statisticsService.DeleteConsensusProjectionsByPosition(_season.CurrentSeason, pos);

            if (pos == Position.FLEX)
            {
                var total = await seasonDataService.UploadConsensusProjections(Position.QB.ToString())
                          + await seasonDataService.UploadConsensusProjections(Position.RB.ToString())
                          + await seasonDataService.UploadConsensusProjections(Position.WR.ToString())
                          + await seasonDataService.UploadConsensusProjections(Position.TE.ToString());
                return Ok(total);
            }
            return Ok(await seasonDataService.UploadConsensusProjections(position));

        }
        [HttpPut("refresh-consensus-weekly-projections/{position}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RefreshConsensusWeeklyProjections(string position)
        {
            if (!Enum.TryParse(position.ToUpper(), out Position pos)) return BadRequest();

            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var ignoreList = await playersService.GetIgnoreList();
            _ = await statisticsService.DeleteConsensusWeeklyProjectionsByPosition(_season.CurrentSeason, currentWeek, pos);

            if (pos == Position.FLEX)
            {
                var total = await weeklyDataService.UploadConsensusWeeklyProjections(currentWeek, Position.QB.ToString(), ignoreList)
                          + await weeklyDataService.UploadConsensusWeeklyProjections(currentWeek, Position.RB.ToString(), ignoreList)
                          + await weeklyDataService.UploadConsensusWeeklyProjections(currentWeek, Position.WR.ToString(), ignoreList)
                          + await weeklyDataService.UploadConsensusWeeklyProjections(currentWeek, Position.TE.ToString(), ignoreList)
                          + await weeklyDataService.UploadConsensusWeeklyProjections(currentWeek, Position.DST.ToString(), ignoreList)
                          + await weeklyDataService.UploadConsensusWeeklyProjections(currentWeek, Position.K.ToString(), ignoreList);
                return Ok(total);
            }
            return Ok(await weeklyDataService.UploadConsensusWeeklyProjections(currentWeek, position, ignoreList));

        }

        [HttpPost("sleeper-map")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> UploadSleeperPlayerMap() => Ok(await playersService.UploadSleeperPlayerMap());

        [HttpPost("season-info")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostSeasonInfo([FromBody] SeasonInfoModel season) => Ok(await playersService.PostSeasonInfo(mapper.Map<SeasonInfo>(season)));
    }
 





}
