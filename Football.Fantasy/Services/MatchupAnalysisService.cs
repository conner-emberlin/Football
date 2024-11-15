using Football.Enums;
using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;

namespace Football.Fantasy.Services
{
    public class MatchupAnalysisService(IPlayersService playersService, IFantasyDataService fantasyDataService,
        IOptionsMonitor<Season> season, IMatchupAnalysisRepository matchupAnalysisRepository, ITeamsService teamsService, ILogger logger) : IMatchupAnalysisService
    {
        private readonly Season _season = season.CurrentValue;

        public Task<List<MatchupRanking>> GetPositionalMatchupRankingsFromSQL(Position position, int season, int week) => matchupAnalysisRepository.GetPositionalMatchupRankingsFromSQL(position.ToString(), season, week);
        public async Task<int> PostMatchupRankings(Position position, int week = 0)
        {
            logger.Information("Uploading Matchup Rankings for position {0} week {1}", position.ToString(), week);
            if (week == 1)
            {
                var weekOneRankings = await GetPositionalMatchupRankingsFromSQL(position, _season.CurrentSeason - 1, await playersService.GetWeeksBySeason(_season.CurrentSeason - 1) + 1);
                var seasonGames = await playersService.GetGamesBySeason(_season.CurrentSeason - 1);

                foreach (var w in weekOneRankings)
                {
                    w.Season = _season.CurrentSeason;
                    w.Week = week;
                    w.GamesPlayed = 0;
                    w.AvgPointsAllowed = w.PointsAllowed / seasonGames;
                }
                return await matchupAnalysisRepository.PostMatchupRankings(weekOneRankings);
            }
            var recordsAdded = await matchupAnalysisRepository.PostMatchupRankings(await CalculatePositionalMatchupRankings(position, week));
            logger.Information("Upload complete. {0} records uploaded", recordsAdded);
            return recordsAdded;
        }

        public async Task<int> GetMatchupRanking(int playerId)
        {
            var team = await teamsService.GetPlayerTeam(_season.CurrentSeason, playerId);
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            if (team != null && currentWeek < await playersService.GetCurrentSeasonWeeks())
            {           
                var opponentId = (await teamsService.GetTeamGames(team.TeamId)).First(g => g.Week == currentWeek).OpposingTeamId;
                if (opponentId > 0)
                {
                    var player = await playersService.GetPlayer(playerId);
                    _ = Enum.TryParse(player.Position, out Position position);
                    var matchupRankings = await GetPositionalMatchupRankingsFromSQL(position, _season.CurrentSeason, currentWeek);
                    var matchupRanking = matchupRankings.FindIndex(m => m.TeamId == opponentId) + 1;
                    return matchupRanking;
                }
            }
            return 0;
        }

        public async Task<List<WeeklyFantasy>> GetTopOpponents(Position position, int teamId)
        {
            List<WeeklyFantasy> opponentFantasy = [];
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var schedule = (await teamsService.GetTeamGames(teamId)).Where(g => g.Week < currentWeek && g.OpposingTeamId > 0);
            foreach (var s in schedule)
            {
                var oppFantasy = (await fantasyDataService.GetWeeklyTeamFantasy(s.OpposingTeam, s.Week))
                                 .Where(f => f.Position == position.ToString())
                                 .OrderByDescending(f => f.FantasyPoints)
                                 .FirstOrDefault();
                if (oppFantasy != null) opponentFantasy.Add(oppFantasy);
            }
            return opponentFantasy;
        }

        public async Task<IEnumerable<MatchupRanking>> GetRestOfSeasonMatchupRankingsByTeam(int teamId)
        {
            List<MatchupRanking> rosRankings = [];
            rosRankings.AddRange(await GetRestOfSeasonMatchupRankingsByTeamAndPosition(teamId, Position.QB));
            rosRankings.AddRange(await GetRestOfSeasonMatchupRankingsByTeamAndPosition(teamId, Position.RB));
            rosRankings.AddRange(await GetRestOfSeasonMatchupRankingsByTeamAndPosition(teamId, Position.WR));
            rosRankings.AddRange(await GetRestOfSeasonMatchupRankingsByTeamAndPosition(teamId, Position.TE));
            rosRankings.AddRange(await GetRestOfSeasonMatchupRankingsByTeamAndPosition(teamId, Position.K));
            rosRankings.AddRange(await GetRestOfSeasonMatchupRankingsByTeamAndPosition(teamId, Position.DST));
            return rosRankings.OrderBy(r => r.Week);
        }

        private async Task<List<MatchupRanking>> GetRestOfSeasonMatchupRankingsByTeamAndPosition(int teamId, Position position)
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var upcomingGames = (await teamsService.GetTeamGames(teamId)).Where(g => g.Week >= currentWeek && g.OpposingTeamId > 0);
            var matchupRankingDictionary = (await GetPositionalMatchupRankingsFromSQL(position, _season.CurrentSeason, currentWeek)).ToDictionary(t => t.TeamId);

            List<MatchupRanking> rosRankings = [];
            foreach (var game in upcomingGames)
            {
                var matchup = matchupRankingDictionary[game.OpposingTeamId];
                matchup.Week = game.Week;
                rosRankings.Add(matchup);
            }

            return rosRankings;
        }
        private async Task<List<MatchupRanking>> CalculatePositionalMatchupRankings(Position position, int week = 0)
        {
            List<MatchupRanking> rankings = [];
            var playersByPosition = (await playersService.GetPlayersByPosition(position)).ToDictionary(p => p.PlayerId);
            var asOfWeek = week > 0 ? week : await playersService.GetCurrentWeek(_season.CurrentSeason);
            var allTeams = await teamsService.GetAllTeams();
            foreach (var team in allTeams)
            {
                double fpTotal = 0;
                var games = await teamsService.GetTeamGames(team.TeamId);
                var filteredGames = games.Where(g => g.Week < asOfWeek && g.OpposingTeamId > 0);
                foreach (var game in filteredGames)
                {
                    var opposingPlayers = await teamsService.GetPlayersByTeam(game.OpposingTeam);
                    foreach (var op in opposingPlayers)
                    {
                        if (playersByPosition.TryGetValue(op.PlayerId, out var player))
                        {
                            var weeklyFantasy = (await fantasyDataService.GetWeeklyFantasy(player.PlayerId, game.OpposingTeam)).FirstOrDefault(w => w.Week == game.Week);
                            if (weeklyFantasy != null) fpTotal += weeklyFantasy.FantasyPoints;
                        }
                    }
                }
                rankings.Add(new MatchupRanking
                {
                    TeamId = team.TeamId,
                    Week = asOfWeek,
                    Season = _season.CurrentSeason,
                    GamesPlayed = filteredGames.Count(),
                    Position = position.ToString(),
                    PointsAllowed = Math.Round(fpTotal, 2),
                    AvgPointsAllowed = filteredGames.Any() ? Math.Round(fpTotal / filteredGames.Count(), 2) : 0
                });
            }
            var matchupRanks = rankings.OrderBy(r => r.AvgPointsAllowed).ToList();
            return matchupRanks;
        }

    }
}
