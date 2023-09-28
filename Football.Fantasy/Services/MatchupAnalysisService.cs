using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Football.Enums;

namespace Football.Fantasy.Services
{
    public class MatchupAnalysisService : IMatchupAnalysisService
    {
        private readonly IPlayersService _playersService;
        private readonly IFantasyDataService _fastaDataService;
        private readonly Season _season;
        private readonly IMemoryCache _cache;

        public MatchupAnalysisService(IPlayersService playersService, IFantasyDataService fantasyDataService, 
            IOptionsMonitor<Season> season, IMemoryCache cache)
        {
            _playersService = playersService;
            _fastaDataService = fantasyDataService;
            _season = season.CurrentValue;
            _cache = cache;
        }
        public async Task<List<MatchupRanking>> PositionalMatchupRankings(PositionEnum position)
        {
            if (RetrieveFromCache(position.ToString()).Any())
            {
                return RetrieveFromCache(position.ToString());
            }
            else
            {
                List<MatchupRanking> rankings = new();
                var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
                foreach (var team in await _playersService.GetAllTeams())
                {
                    double fpTotal = 0;
                    var games = await _playersService.GetTeamGames(team.TeamId);
                    foreach (var game in games.Where(g => g.Week < currentWeek).ToList())
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
                        GamesPlayed = currentWeek - 1,
                        Position = position.ToString(),
                        PointsAllowed = Math.Round(fpTotal, 2),
                        AvgPointsAllowed = currentWeek > 1 ? Math.Round(fpTotal / (currentWeek - 1), 2) : 0
                    });
                }
                var matchupRanks = rankings.OrderBy(r => r.PointsAllowed).ToList();
                _cache.Set("MatchupRanking" + position.ToString(), matchupRanks);
                return matchupRanks;
            }
        }

        private List<MatchupRanking> RetrieveFromCache(string position) =>
            _cache.TryGetValue("MatchupRanking" + position, out List<MatchupRanking> rankings) ? rankings
            : Enumerable.Empty<MatchupRanking>().ToList();
    }
}
