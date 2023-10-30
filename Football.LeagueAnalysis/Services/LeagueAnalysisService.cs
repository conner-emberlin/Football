using Football.Models;
using Football.LeagueAnalysis.Interfaces;
using Football.LeagueAnalysis.Models;
using Football.Players.Models;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;
using Football.Projections.Models;

namespace Football.LeagueAnalysis.Services
{
    public class LeagueAnalysisService : ILeagueAnalysisService
    {
        private readonly ILeagueAnalysisRepository _leagueAnalysisRepository;
        private readonly ISleeperLeagueService _sleeperLeagueService;
        private readonly IPlayersService _playersService;
        private readonly Season _season;

        public LeagueAnalysisService(ILeagueAnalysisRepository leagueAnalysisRepository, ISleeperLeagueService sleeperLeagueService, IOptionsMonitor<Season> season,
            IPlayersService playersService)
        {
            _leagueAnalysisRepository = leagueAnalysisRepository;
            _sleeperLeagueService = sleeperLeagueService;
            _playersService = playersService;
            _season = season.CurrentValue;
        }

        public async Task<int> UploadSleeperPlayerMap()
        {
            var sleeperPlayers =  await _sleeperLeagueService.GetSleeperPlayers();
            var playerMap = await _sleeperLeagueService.GetSleeperPlayerMap(sleeperPlayers);
            return await _leagueAnalysisRepository.UploadSleeperPlayerMap(playerMap);
        }

        public async Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId) => await _leagueAnalysisRepository.GetSleeperPlayerMap(sleeperId);

        public async Task<List<WeekProjection>> GetSleeperLeagueProjections(string username)
        {
            List<WeekProjection> projections = new();           
            var sleeperStarters = await GetSleeperLeagueStarters(username);        
            if (sleeperStarters.Any())
            {
                var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
                foreach (var starter in sleeperStarters)
                {
                    var projection = await _playersService.GetWeeklyProjection(_season.CurrentSeason, currentWeek, starter.PlayerId);
                    projections.Add(new WeekProjection
                    {
                        PlayerId = starter.PlayerId,
                        Season = _season.CurrentSeason,
                        Week = currentWeek,
                        Name = starter.Name,
                        Position = starter.Position,
                        ProjectedPoints = projection
                    });
                }
            }
            return projections;
        }
        private async Task<Tuple<SleeperUser, SleeperLeague?>?> GetCurrentSleeperLeague(string username)
        {
            var sleeperUser = await _sleeperLeagueService.GetSleeperUser(username);
            if (sleeperUser != null)
            {
                var userLeagues = await _sleeperLeagueService.GetSleeperLeagues(sleeperUser.UserId);
                if (userLeagues != null)
                {
                    var league = userLeagues.FirstOrDefault(u => u.Season == _season.CurrentSeason.ToString() && u.Status == "in_season");
                    return Tuple.Create(sleeperUser, league);
                }
                else return null;
            }
            else return null;
        }
        private async Task<List<Player>> GetSleeperLeagueStarters(string username)
        {       
            List<Player> sleeperStarters = new();
            var tuple = await GetCurrentSleeperLeague(username);
            if (tuple != null)
            {
                var (sleeperUser, currentLeague) = tuple;
                if (currentLeague != null)
                {
                    var roster = (await _sleeperLeagueService.GetSleeperRosters(currentLeague.LeagueId)).FirstOrDefault(r => r.OwnerId == sleeperUser.UserId);
                    if (roster != null)
                    {
                        foreach (var starter in roster.Starters)
                        {
                            if (int.TryParse(starter, out var sleeperId))
                            {
                                var sleeperMap = await GetSleeperPlayerMap(sleeperId);
                                if (sleeperMap != null)
                                {
                                    sleeperStarters.Add(await _playersService.GetPlayer(sleeperMap.PlayerId));
                                }
                            }
                        }
                    }
                }
            }
            return sleeperStarters;
        }

    }
}
