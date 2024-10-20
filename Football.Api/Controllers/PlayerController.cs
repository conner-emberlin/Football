using Microsoft.AspNetCore.Mvc;
using Football.Models;
using Football.Enums;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;
using AutoMapper;
using Football.Fantasy.Interfaces;
using Football.Shared.Models.Players;
using Football.Shared.Models.Fantasy;

namespace Football.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController(IPlayersService playersService, IStatisticsService statisticsService, IOptionsMonitor<Season> season, IMapper mapper, IFantasyDataService fantasyDataService, IFantasyAnalysisService fantasyAnalysisService) : ControllerBase
    {
        private readonly Season _season = season.CurrentValue;

        [HttpGet("data/players")]
        [ProducesResponseType(typeof(List<SimplePlayerModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPlayers([FromQuery] int active, [FromQuery] string position = "") 
        {
            var allPlayers = await playersService.GetAllPlayers();
            var filteredPlayers =  
                                  active == 1 && position != string.Empty ? allPlayers.Where(p => p.Active == 1 && p.Position == position)
                                : active == 1 && position == string.Empty ? allPlayers.Where(p => p.Active == 1)
                                : position != string.Empty && active != 1 ? allPlayers.Where(p => p.Position == position)
                                : allPlayers;

            return Ok(mapper.Map<List<SimplePlayerModel>>(filteredPlayers.OrderBy(p => p.Name)));
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

            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var season = 0;
            if (currentWeek == 1)
            {
                season = _season.CurrentSeason - 1;
                playerModel.WeeklyDataFromPastSeason = true;
            }

            else
            {
                season = _season.CurrentSeason;
                playerModel.WeeklyDataFromPastSeason = false;
            }

            playerModel.WeeklyData = (position switch
            {
                Position.QB => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyDataByPlayer<WeeklyDataQB>(position, playerId, season)),
                Position.WR => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyDataByPlayer<WeeklyDataWR>(position, playerId, season)),
                Position.RB => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyDataByPlayer<WeeklyDataRB>(position, playerId, season)),
                Position.TE => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyDataByPlayer<WeeklyDataTE>(position, playerId, season)),
                Position.DST => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyDataByPlayer<WeeklyDataDST>(position, playerId, season)),
                Position.K => mapper.Map<List<WeeklyDataModel>>(await statisticsService.GetWeeklyDataByPlayer<WeeklyDataK>(position, playerId, season)),
                _ => []
            }).OrderByDescending(w => w.Week).ToList();

            playerModel.SeasonData = position switch
            {
                Position.QB => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataQB>(position, playerId, true)),
                Position.WR => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataWR>(position, playerId, true)),
                Position.RB => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataRB>(position, playerId, true)),
                Position.TE => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataTE>(position, playerId, true)),
                Position.DST => mapper.Map<List<SeasonDataModel>>(await statisticsService.GetSeasonData<SeasonDataDST>(position, playerId, true)),
                _ => []
            };

            playerModel.SeasonFantasy = mapper.Map<List<SeasonFantasyModel>>(await fantasyDataService.GetSeasonFantasy(playerId));
            playerModel.WeeklyFantasy = mapper.Map<List<WeeklyFantasyModel>>(await fantasyDataService.GetWeeklyFantasyBySeason(playerId, season));

            if (playerModel.WeeklyFantasy.Count > 0) 
            {
                var currentTotals = await fantasyDataService.GetCurrentFantasyTotals(season);
                playerModel.RunningFantasyTotal = currentTotals.First(c => c.PlayerId == playerId).FantasyPoints;
                playerModel.OverallRank = currentTotals.Select(p => p.PlayerId).ToList().IndexOf(playerId) + 1;
                playerModel.PositionRank = currentTotals.Where(c => c.Position == player.Position).Select(p => p.PlayerId).ToList().IndexOf(playerId) + 1;

                playerModel.WeeklyFantasyTrends = mapper.Map<List<WeeklyFantasyTrendModel>>(await fantasyAnalysisService.GetPlayerWeeklyFantasyTrends(playerId, season));
            }
           
            return Ok(playerModel);
        }

        [HttpGet("simple/{playerId}")]
        [ProducesResponseType(typeof(SimplePlayerModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSimplePlayer(int playerId) => Ok(mapper.Map<SimplePlayerModel>(await playersService.GetPlayer(playerId)));


        [HttpPost("add")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePlayer([FromBody] SimplePlayerModel player) => Ok(await playersService.CreatePlayer(player.Name, player.Active, player.Position));

        [HttpPost("injury")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostInSeasonInjury([FromBody] InSeasonInjuryModel injury) => Ok(await playersService.PostInSeasonInjury(mapper.Map<InSeasonInjury>(injury)));

        [HttpGet("player-injuries")]
        [ProducesResponseType(typeof(List<PlayerInjuryModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPlayerInjuries() => Ok(mapper.Map<List<PlayerInjuryModel>>(await playersService.GetPlayerInjuries()));

        [HttpPut("update-injury")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateInjury([FromBody] InSeasonInjuryModel injury) => Ok(await playersService.UpdateInjury(mapper.Map<InSeasonInjury>(injury)));


        [HttpPost("team-change/in-season")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostInSeasonTeamChange([FromBody] InSeasonTeamChangeModel teamChange)
        {
            var allTeamsDictionary = (await playersService.GetAllTeams()).Select(t => t.Team);
            var allPlayers = (await playersService.GetAllPlayers()).Select(p => p.PlayerId);

            if (teamChange.WeekEffective <= 0) return BadRequest();
            if (!allTeamsDictionary.Contains(teamChange.PreviousTeam) || !allTeamsDictionary.Contains(teamChange.NewTeam) || !allPlayers.Contains(teamChange.PlayerId)) return NotFound();

            var tc = mapper.Map<InSeasonTeamChange>(teamChange);
            tc.Season = _season.CurrentSeason;

            return Ok(await playersService.PostInSeasonTeamChange(tc));
        }

        [HttpGet("team-changes")]
        [ProducesResponseType(typeof(List<InSeasonTeamChangeModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInSeasonTeamChanges() 
        {
            var teamChanges = mapper.Map<List<InSeasonTeamChangeModel>>(await playersService.GetInSeasonTeamChanges());

            if (teamChanges.Count > 0)
            {
                var nameDictionary = (await playersService.GetAllPlayers()).ToDictionary(p => p.PlayerId, p => p.Name);
                teamChanges.ForEach(t => t.Name = nameDictionary.TryGetValue(t.PlayerId, out var name) ? name : string.Empty);
            }

            return Ok(teamChanges);
        } 


        [HttpGet("snap-counts/{playerId}")]
        [ProducesResponseType(typeof(List<SnapCountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSnapCounts([FromRoute] int playerId) => Ok(mapper.Map<List<SnapCountModel>>(await statisticsService.GetSnapCounts(playerId)));

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

        [HttpGet("division-standings/{division}")]
        [ProducesResponseType(typeof(List<DivisionStanding>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStandingsByDivision(string division)
        {
            if (!Enum.TryParse<Division>(division, out var divisionEnum)) return NotFound();

            return Ok(await statisticsService.GetStandingsByDivision(divisionEnum));
        }
    }
}
