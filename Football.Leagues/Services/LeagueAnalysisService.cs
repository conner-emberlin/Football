using Football.Models;
using Football.Enums;
using Football.Players.Models;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;
using Football.Projections.Models;
using Football.Leagues.Interfaces;
using Football.Leagues.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Football.Leagues.Services
{
    public class LeagueAnalysisService(ILeagueAnalysisRepository leagueAnalysisRepository, ISleeperLeagueService sleeperLeagueService, IOptionsMonitor<Season> season,
        IPlayersService playersService, ISettingsService settingsService, IMemoryCache cache) : ILeagueAnalysisService
    {
        private readonly Season _season = season.CurrentValue;

        public async Task<int> UploadSleeperPlayerMap()
        {
            var sleeperPlayers = await sleeperLeagueService.GetSleeperPlayers();
            var existingMaps = await leagueAnalysisRepository.GetSleeperPlayerMaps();
            var playerMap = (await GetSleeperPlayerMap(sleeperPlayers)).Where(s => !existingMaps.Any(e => e.PlayerId == s.PlayerId)).ToList();
            return await leagueAnalysisRepository.UploadSleeperPlayerMap(playerMap);
        }

        public async Task<List<MatchupProjections>> GetMatchupProjections(string username, int week)
        {
            List<MatchupProjections> matchupProjections = [];
            var leagueTuple = await GetCurrentSleeperLeague(username);
            if (leagueTuple is not null)
            {
                var (user, league) = leagueTuple;
                if (league is not null)
                {
                    var rosters = await sleeperLeagueService.GetSleeperRosters(league.LeagueId);
                    var matchups = await sleeperLeagueService.GetSleeperMatchups(league.LeagueId, week);
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
                        var opponent = await sleeperLeagueService.GetSleeperUser(opponentRoster.OwnerId);
                        matchupProjections.Add( new MatchupProjections {LeagueName = league.Name, TeamName = user.DisplayName, TeamProjections = userProjections });
                        matchupProjections.Add(new MatchupProjections { LeagueName = league.Name, TeamName = opponent!.DisplayName, TeamProjections = opponentProjections });
                    }
                }
            }
            return matchupProjections;
        }

        public async Task<List<WeekProjection>> GetSleeperLeagueProjections(string username)
        {
            List<WeekProjection> projections = [];
            var sleeperStarters = await GetSleeperLeagueStarters(username);
            if (sleeperStarters.Count > 0)
            {
                var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
                foreach (var starter in sleeperStarters)
                {
                    var projection = await playersService.GetWeeklyProjection(_season.CurrentSeason, currentWeek, starter.PlayerId);
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

        public async Task<List<TrendingPlayer>> GetTrendingPlayers()
        {
            if (settingsService.GetFromCache<TrendingPlayer>(Cache.TrendingPlayers, out var cachedTrending))
            {
                return cachedTrending;
            }
            else
            {
                var sleeperTrendingPlayers = await sleeperLeagueService.GetSleeperTrendingPlayers();
                List<TrendingPlayer> trendingPlayers = [];

                if (sleeperTrendingPlayers != null)
                {
                    foreach (var sleeperPlayer in sleeperTrendingPlayers)
                    {
                        if (int.TryParse(sleeperPlayer.SleeperPlayerId, out var sleeperId))
                        {
                            var sleeperMap = await GetSleeperPlayerMap(sleeperId);
                            if (sleeperMap != null)
                            {
                                trendingPlayers.Add(new TrendingPlayer
                                {
                                    Player = await playersService.GetPlayer(sleeperMap.PlayerId),
                                    PlayerTeam = await playersService.GetPlayerTeam(_season.CurrentSeason, sleeperMap.PlayerId),
                                    Adds = sleeperPlayer.Adds
                                });
                            }

                        }
                    }
                }
                cache.Set(Cache.TrendingPlayers.ToString(), trendingPlayers);
                return trendingPlayers;
            }
        }
        private async Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId) => await leagueAnalysisRepository.GetSleeperPlayerMap(sleeperId);
        private async Task<List<SleeperPlayerMap>> GetSleeperPlayerMaps() => await leagueAnalysisRepository.GetSleeperPlayerMaps();
        private async Task<Tuple<SleeperUser, SleeperLeague?>?> GetCurrentSleeperLeague(string username)
        {
            var sleeperUser = await sleeperLeagueService.GetSleeperUser(username);
            if (sleeperUser is not null)
            {
                var userLeagues = await sleeperLeagueService.GetSleeperLeagues(sleeperUser.UserId);
                if (userLeagues is not null)
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
            List<Player> sleeperStarters = [];
            var tuple = await GetCurrentSleeperLeague(username);
            if (tuple is not null)
            {
                var (sleeperUser, currentLeague) = tuple;
                if (currentLeague is not null)
                {
                    var roster = (await sleeperLeagueService.GetSleeperRosters(currentLeague.LeagueId))!
                                .FirstOrDefault(r => r.OwnerId == sleeperUser.UserId);
                    if (roster is not null)
                    {
                        foreach (var starter in roster.Starters)
                        {
                            if (int.TryParse(starter, out var sleeperId))
                            {
                                var sleeperMap = await GetSleeperPlayerMap(sleeperId);
                                if (sleeperMap is not null)
                                {
                                    sleeperStarters.Add(await playersService.GetPlayer(sleeperMap.PlayerId));
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
            List<WeekProjection> projections = [];
            foreach (var starter in roster.Starters)
            {
                if (int.TryParse(starter, out var sleeperId))
                {
                    var sleeperMap = await GetSleeperPlayerMap(sleeperId);
                    if (sleeperMap is not null)
                    {
                        var player = await playersService.GetPlayer(sleeperMap.PlayerId);
                        var projection = await playersService.GetWeeklyProjection(_season.CurrentSeason, week, player.PlayerId);
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
            List<SleeperPlayerMap> playerMap = [];
            if (sleeperPlayers.Count > 0)
            {
                foreach (var sp in sleeperPlayers)
                {
                    if (!string.IsNullOrWhiteSpace(sp.PlayerName))
                    {
                        var playerId = await playersService.GetPlayerId(sp.PlayerName);
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
