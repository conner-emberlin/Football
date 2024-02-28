using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Analysis.Models;
using Football.Fantasy.Analysis.Interfaces;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Football.Enums;
using Football.Players.Models;
using Football.Fantasy.Models;

namespace Football.Fantasy.Analysis.Services
{
    public class MatchupAnalysisService(IPlayersService playersService, IFantasyDataService fantasyDataService,
        IOptionsMonitor<Season> season, IMemoryCache cache, ISettingsService settings) : IMatchupAnalysisService
    {
        private readonly Season _season = season.CurrentValue;

        public async Task<List<MatchupRanking>> PositionalMatchupRankings(Position position)
        {
            if (settings.GetFromCache<MatchupRanking>(position, Cache.MatchupRankings, out var cachedValues))
                return cachedValues;
            else
            {
                List<MatchupRanking> rankings = [];
                var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
                foreach (var team in await playersService.GetAllTeams())
                {
                    double fpTotal = 0;
                    var games = await playersService.GetTeamGames(team.TeamId);
                    var filteredGames = games.Where(g => g.Week < currentWeek && g.OpposingTeamId > 0).ToList();
                    foreach (var game in filteredGames)
                    {
                        var opposingPlayers = await playersService.GetPlayersByTeam(game.OpposingTeam);
                        foreach (var op in opposingPlayers)
                        {
                            var player = await playersService.GetPlayer(op.PlayerId);
                            if (player.Position == position.ToString())
                            {
                                var weeklyFantasy = (await fantasyDataService.GetWeeklyFantasy(player.PlayerId, game.OpposingTeam)).Where(w => w.Week == game.Week).FirstOrDefault();
                                if (weeklyFantasy != null)
                                    fpTotal += weeklyFantasy.FantasyPoints;
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
                cache.Set(position.ToString() + Cache.MatchupRankings, matchupRanks);
                return matchupRanks;
            }
        }

        public async Task<int> GetMatchupRanking(int playerId)
        {
            var team = await playersService.GetPlayerTeam(_season.CurrentSeason, playerId);
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            if (team != null && currentWeek < _season.Weeks)
            {
                var teamId = await playersService.GetTeamId(team.Team);               
                var opponentId = (await playersService.GetTeamGames(teamId)).Where(g => g.Week == currentWeek).First().OpposingTeamId;
                var player = await playersService.GetPlayer(playerId);
                _ = Enum.TryParse(player.Position, out Position position);
                var matchupRankings = await PositionalMatchupRankings(position);
                var matchupRanking = matchupRankings.FindIndex(m => m.Team.TeamId == opponentId) + 1;
                return matchupRanking;
            }
            else return 0;
        }

        public async Task<List<WeeklyFantasy>> GetTopOpponents(Position position, int teamId)
        {
            List<WeeklyFantasy> opponentFantasy = [];
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var schedule = (await playersService.GetTeamGames(teamId)).Where(g => g.Week < currentWeek && g.OpposingTeam != "BYE");
            foreach (var s in schedule)
            {
                var oppFantasy = (await fantasyDataService.GetWeeklyTeamFantasy(s.OpposingTeam, s.Week))
                                 .Where(f => f.Position == position.ToString())
                                 .OrderByDescending(f => f.FantasyPoints)
                                 .FirstOrDefault();
                if (oppFantasy != null)
                    opponentFantasy.Add(oppFantasy);                
            }
            return opponentFantasy;
        }
    }
}
