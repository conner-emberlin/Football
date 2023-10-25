using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Analysis.Models;
using Football.Fantasy.Analysis.Interfaces;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Football.Enums;

namespace Football.Fantasy.Analysis.Services
{
    public class MatchupAnalysisService : IMatchupAnalysisService
    {
        private readonly IPlayersService _playersService;
        private readonly IFantasyDataService _fastaDataService;
        private readonly Season _season;
        private readonly IMemoryCache _cache;
        private readonly ISettingsService _settings;

        public MatchupAnalysisService(IPlayersService playersService, IFantasyDataService fantasyDataService, 
            IOptionsMonitor<Season> season, IMemoryCache cache, ISettingsService settings)
        {
            _playersService = playersService;
            _fastaDataService = fantasyDataService;
            _season = season.CurrentValue;
            _cache = cache;
            _settings = settings;
        }
        public async Task<List<MatchupRanking>> PositionalMatchupRankings(PositionEnum position)
        {
            if (_settings.GetFromCache<MatchupRanking>(position, Cache.MatchupRankings, out var cachedValues))
            {
                return cachedValues;
            }
            else
            {
                List<MatchupRanking> rankings = new();
                var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
                foreach (var team in await _playersService.GetAllTeams())
                {
                    double fpTotal = 0;
                    var games = await _playersService.GetTeamGames(team.TeamId);
                    var filteredGames = games.Where(g => g.Week < currentWeek && g.OpposingTeamId > 0).ToList();
                    foreach (var game in filteredGames)
                    {
                        var opposingPlayers = await _playersService.GetPlayersByTeam(game.OpposingTeam);
                        foreach (var op in opposingPlayers)
                        {
                            var player = await _playersService.GetPlayer(op.PlayerId);
                            if (player.Position == position.ToString())
                            {
                                var weeklyFantasy = (await _fastaDataService.GetWeeklyFantasy(player.PlayerId)).Where(w => w.Week == game.Week).FirstOrDefault();
                                if (weeklyFantasy != null)
                                {
                                    fpTotal += weeklyFantasy.FantasyPoints;
                                }
                            }
                        }
                    }
                    rankings.Add(new MatchupRanking
                    {
                        Team = team,
                        GamesPlayed = filteredGames.Count,
                        Position = position.ToString(),
                        PointsAllowed = Math.Round(fpTotal, 2),
                        AvgPointsAllowed = filteredGames.Count > 1 ? Math.Round(fpTotal / filteredGames.Count, 2) : 0
                    });
                }
                var matchupRanks = rankings.OrderBy(r => r.AvgPointsAllowed).ToList();
                _cache.Set(position.ToString() + Cache.MatchupRankings, matchupRanks);
                return matchupRanks;
            }
        }

        public async Task<int> GetMatchupRanking(int playerId)
        {
            var team = await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId);
            if (team != null)
            {
                var teamId = await _playersService.GetTeamId(team.Team);
                var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
                var opponentId = (await _playersService.GetTeamGames(teamId)).Where(g => g.Week == currentWeek).First().OpposingTeamId;
                var player = await _playersService.GetPlayer(playerId);
                _ = Enum.TryParse(player.Position, out PositionEnum position);
                var matchupRankings = await PositionalMatchupRankings(position);
                var matchupRanking = matchupRankings.FindIndex(m => m.Team.TeamId == opponentId) + 1;
                return matchupRanking;
            }
            else return 0;
        }
    }
}
