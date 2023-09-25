using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;

namespace Football.Fantasy.Services
{
    public class MatchupAnalysisService : IMatchupAnalysisService
    {
        private readonly IPlayersService _playersService;
        private readonly IFantasyDataService _fastaDataService;
        private readonly Season _season;

        public MatchupAnalysisService(IPlayersService playersService, IFantasyDataService fantasyDataService, IOptionsMonitor<Season> season)
        {
            _playersService = playersService;
            _fastaDataService = fantasyDataService;
            _season = season.CurrentValue;
        }
        public async Task<List<MatchupRanking>> PositionalMatchupRankings(string position)
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
                        if (player.Position == position)
                        {
                            var weeklyFantasy = (await _fastaDataService.GetWeeklyFantasy(player.PlayerId)).Where(w => w.Week == game.Week).FirstOrDefault();
                            if(weeklyFantasy != null)
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
                    Position = position,
                    PointsAllowed = Math.Round(fpTotal, 2),
                    AvgPointsAllowed = currentWeek > 1 ? Math.Round(fpTotal/(currentWeek - 1),2) : 0
                });
            }
            return rankings.OrderBy(r => r.PointsAllowed).ToList();
        }
    }
}
