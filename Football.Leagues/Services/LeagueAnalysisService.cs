using Football.Models;
using Football.Players.Models;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;
using Football.Projections.Models;
using Football.Leagues.Interfaces;
using Football.Leagues.Models;

namespace Football.Leagues.Services
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
            var sleeperPlayers = await _sleeperLeagueService.GetSleeperPlayers();
            var playerMap = await GetSleeperPlayerMap(sleeperPlayers);
            return await _leagueAnalysisRepository.UploadSleeperPlayerMap(playerMap);
        }

        public async Task<List<MatchupProjections>> GetMatchupProjections(string username, int week)
        {
            List<MatchupProjections> matchupProjections = new();
            var leagueTuple = await GetCurrentSleeperLeague(username);
            if (leagueTuple != null)
            {
                var (user, league) = leagueTuple;
                if (league != null)
                {
                    var rosters = await _sleeperLeagueService.GetSleeperRosters(league.LeagueId);
                    var matchups = await _sleeperLeagueService.GetSleeperMatchups(league.LeagueId, week);
                    if (rosters != null && matchups != null)
                    {
                        var rosterMatchup = rosters.Join(matchups, r => r.RosterId, m => m.RosterId, (r, m) => new { SleeperRoster = r, SleeperMatchup = m })
                                           .GroupBy(rm => rm.SleeperMatchup.MatchupId,
                                                    rm => rm.SleeperRoster,
                                                    (key, r) => new { MatchupId = key, Rosters = r.ToList() })
                                           .First(g => g.Rosters.Any(r => r.OwnerId == user.UserId));

                        var userRoster = rosterMatchup.Rosters.First(r => r.OwnerId == user.UserId);
                        var userProjections = await GetProjectionsFromRoster(userRoster, week);
                        var opponentRoster = rosterMatchup.Rosters.First(r => r.OwnerId != user.UserId);
                        var opponentProjections = await GetProjectionsFromRoster(opponentRoster, week);
                        var opponent = await _sleeperLeagueService.GetSleeperUser(opponentRoster.OwnerId);
                        matchupProjections.Add( new MatchupProjections {LeagueName = league.Name, TeamName = user.DisplayName, TeamProjections = userProjections });
                        matchupProjections.Add(new MatchupProjections { LeagueName = league.Name, TeamName = opponent!.DisplayName, TeamProjections = opponentProjections });
                    }
                }
            }
            return matchupProjections;
        }

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
        private async Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId) => await _leagueAnalysisRepository.GetSleeperPlayerMap(sleeperId);
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
                    var roster = (await _sleeperLeagueService.GetSleeperRosters(currentLeague.LeagueId))!
                                .FirstOrDefault(r => r.OwnerId == sleeperUser.UserId);
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
        private async Task<List<WeekProjection>> GetProjectionsFromRoster(SleeperRoster roster, int week)
        {
            List<WeekProjection> projections = new();
            foreach (var starter in roster.Starters)
            {
                if (int.TryParse(starter, out var sleeperId))
                {
                    var sleeperMap = await GetSleeperPlayerMap(sleeperId);
                    if (sleeperMap != null)
                    {
                        var player = await _playersService.GetPlayer(sleeperMap.PlayerId);
                        var projection = await _playersService.GetWeeklyProjection(_season.CurrentSeason, week, player.PlayerId);
                        projections.Add(new WeekProjection
                        {
                            PlayerId = player.PlayerId,
                            Season = _season.CurrentSeason,
                            Week = week,
                            Name = player.Name,
                            Position = player.Position,
                            ProjectedPoints = projection
                        });
                    }
                }
            }
            return projections;
        }
        private async Task<List<SleeperPlayerMap>> GetSleeperPlayerMap(List<SleeperPlayer> sleeperPlayers)
        {
            List<SleeperPlayerMap> playerMap = new();
            if (sleeperPlayers.Any())
            {
                foreach (var sp in sleeperPlayers)
                {
                    if (!string.IsNullOrWhiteSpace(sp.PlayerName))
                    {
                        var playerId = await _playersService.GetPlayerId(sp.PlayerName);
                        if (playerId > 0)
                        {
                            playerMap.Add(new SleeperPlayerMap
                            {
                                SleeperPlayerId = sp.SleeperPlayerId,
                                PlayerId = playerId
                            });
                        }
                    }
                }
                return playerMap;
            }
            else throw new NullReferenceException();
        }
    }
}
