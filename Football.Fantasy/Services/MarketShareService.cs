using Football.Fantasy.Interfaces;
using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;
using Football.Fantasy.Models;
using Microsoft.Extensions.Caching.Memory;
using Football.Players.Models;

namespace Football.Fantasy.Services
{
    public class MarketShareService : IMarketShareService
    {
        private readonly IPlayersService _playersService;
        private readonly IStatisticsService _statisticsService;
        private readonly IFantasyDataService _fantasyService;
        private readonly IMemoryCache _cache;
        private readonly Season _season;

        public MarketShareService(IPlayersService playersService, IStatisticsService statisticsService, IFantasyDataService fantasyService,
            IOptionsMonitor<Season> season, IMemoryCache cache)
        {
            _playersService = playersService;
            _statisticsService = statisticsService;
            _fantasyService = fantasyService;
            _cache = cache;
            _season = season.CurrentValue;
        }
        public async Task<List<TargetShare>> GetTargetShares()
        {
            var teams = await _playersService.GetAllTeams();
            List<TargetShare> shares = new();
            foreach (var team in teams)
            {
                var allPlayers = await _playersService.GetPlayersByTeam(team.Team);
                List<Player> players = new();

                var totalAttempts = 0.0;
                var totalCompletions = 0.0;
                var RBTargets = 0.0;
                var RBComps = 0.0;
                var WRTargets = 0.0;
                var WRComps = 0.0;
                var TETargets = 0.0;
                var TEComps = 0.0;

                foreach (var p in allPlayers)
                {
                    _ = Enum.TryParse((await _playersService.GetPlayer(p.PlayerId)).Position, out PositionEnum position);
                    if (position == PositionEnum.QB)
                    {
                        var stats = await _statisticsService.GetWeeklyDataQB(p.PlayerId);
                        totalAttempts += stats.Sum(s => s.Attempts);
                        totalCompletions += stats.Sum(s => s.Completions);
                    }
                    else if (position == PositionEnum.RB)
                    {
                        var stats = await _statisticsService.GetWeeklyDataRB(p.PlayerId);
                        RBTargets += stats.Sum(s => s.Targets);
                        RBComps += stats.Sum(s => s.Receptions);
                    }
                    else if (position == PositionEnum.WR)
                    {
                        var stats = await _statisticsService.GetWeeklyDataWR(p.PlayerId);
                        WRTargets += stats.Sum(s => s.Targets);
                        WRComps += stats.Sum(s => s.Receptions);
                    }
                    else if (position == PositionEnum.TE)
                    {
                        var stats = await _statisticsService.GetWeeklyDataTE(p.PlayerId);
                        TETargets += stats.Sum(s => s.Targets);
                        TEComps += stats.Sum(s => s.Receptions);
                    }
                }
                shares.Add(new TargetShare
                {
                    Team = team,
                    RBTargetShare = totalAttempts > 0 ? Math.Round(RBTargets / totalAttempts, 3) : 0,
                    RBCompShare = totalCompletions > 0 ? Math.Round(RBComps / totalCompletions, 3) : 0,
                    WRTargetShare = totalAttempts > 0 ? Math.Round(WRTargets / totalAttempts, 3) : 0,
                    WRCompShare = totalCompletions > 0 ? Math.Round(WRComps / totalCompletions, 3) : 0,
                    TETargetShare = totalAttempts > 0 ? Math.Round(TETargets / totalAttempts, 3) : 0,
                    TECompShare = totalCompletions > 0 ? Math.Round(TEComps / totalCompletions, 3) : 0
                });
            }
            return shares.OrderBy(s => s.Team.TeamDescription).ToList();
        }
        public async Task<List<MarketShare>> GetMarketShare(PositionEnum position)
        {
            List<MarketShare> share = new();
            var players = (await _playersService.GetPlayersByPosition(position)).Where(p => p.Active == 1).ToList();
            var teamTotals = await GetTeamTotals();
            foreach (var player in players)
            {
                var team = await _playersService.GetPlayerTeam(_season.CurrentSeason, player.PlayerId);
                if (team != null) {
                    var fantasy = await _fantasyService.GetWeeklyFantasy(player.PlayerId);
                    var teamTotal = teamTotals.Where(t => t.Team.Team == team.Team).First();
                    if (position == PositionEnum.RB)
                    {
                        var stats = await _statisticsService.GetWeeklyDataRB(player.PlayerId);
                        if (stats.Any())
                        {
                            share.Add(new MarketShare
                            {
                                Player = player,
                                PlayerTeam = team,
                                Games = teamTotals.First().Games,
                                TotalFantasy = fantasy.Sum(f => f.FantasyPoints),
                                FantasyShare = teamTotal.TotalFantasyRB > 0 ? fantasy.Sum(f => f.FantasyPoints) / teamTotal.TotalFantasyRB : 0,
                                RushAttShare = teamTotal.TotalRushAtt > 0 ? stats.Sum(s => s.RushingAtt) / teamTotal.TotalRushAtt : 0,
                                RushYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.RushingYds) / teamTotal.TotalRecYds : 0,
                                RushTDShare = teamTotal.TotalRushTd > 0 ? stats.Sum(s => s.RushingTD) / teamTotal.TotalRushTd : 0,
                                TargetShare = teamTotal.TotalTargets > 0 ? stats.Sum(s => s.Targets) / teamTotal.TotalTargets : 0,
                                ReceptionShare = teamTotal.TotalReceptions > 0 ? stats.Sum(s => s.Receptions) / teamTotal.TotalReceptions : 0,
                                RecYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.Yards) / teamTotal.TotalRecYds : 0,
                                RecTDShare = teamTotal.TotalRecTd > 0 ? stats.Sum(s => s.ReceivingTD) / teamTotal.TotalRecTd : 0
                            });
                        }
                    }
                    else if (position == PositionEnum.WR)
                    {
                        var stats = await _statisticsService.GetWeeklyDataWR(player.PlayerId);
                        if (stats.Any())
                        {
                            share.Add(new MarketShare
                            {
                                Player = player,
                                PlayerTeam = team,
                                Games = teamTotals.First().Games,
                                TotalFantasy = fantasy.Sum(f => f.FantasyPoints),
                                FantasyShare = teamTotal.TotalFantasyWR > 0 ? fantasy.Sum(f => f.FantasyPoints) / teamTotal.TotalFantasyWR : 0,
                                RushAttShare = teamTotal.TotalRushAtt > 0 ? stats.Sum(s => s.RushingAtt) / teamTotal.TotalRushAtt : 0,
                                RushYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.RushingYds) / teamTotal.TotalRecYds : 0,
                                RushTDShare = teamTotal.TotalRushTd > 0 ? stats.Sum(s => s.RushingTD) / teamTotal.TotalRushTd : 0,
                                TargetShare = teamTotal.TotalTargets > 0 ? stats.Sum(s => s.Targets) / teamTotal.TotalTargets : 0,
                                ReceptionShare = teamTotal.TotalReceptions > 0 ? stats.Sum(s => s.Receptions) / teamTotal.TotalReceptions : 0,
                                RecYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.Yards) / teamTotal.TotalRecYds : 0,
                                RecTDShare = teamTotal.TotalRecTd > 0 ? stats.Sum(s => s.TD) / teamTotal.TotalRecTd : 0
                            });
                        }
                    }
                    else if (position == PositionEnum.TE)
                    {
                        var stats = await _statisticsService.GetWeeklyDataTE(player.PlayerId);
                        if (stats.Any())
                        {
                            share.Add(new MarketShare
                            {
                                Player = player,
                                PlayerTeam = team,
                                Games = teamTotals.First().Games,
                                TotalFantasy = fantasy.Sum(f => f.FantasyPoints),
                                FantasyShare = teamTotal.TotalFantasyTE > 0 ? fantasy.Sum(f => f.FantasyPoints) / teamTotal.TotalFantasyTE : 0,
                                RushAttShare = teamTotal.TotalRushAtt > 0 ? stats.Sum(s => s.RushingAtt) / teamTotal.TotalRushAtt : 0,
                                RushYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.RushingYds) / teamTotal.TotalRecYds : 0,
                                RushTDShare = teamTotal.TotalRushTd > 0 ? stats.Sum(s => s.RushingTD) / teamTotal.TotalRushTd : 0,
                                TargetShare = teamTotal.TotalTargets > 0 ? stats.Sum(s => s.Targets) / teamTotal.TotalTargets : 0,
                                ReceptionShare = teamTotal.TotalReceptions > 0 ? stats.Sum(s => s.Receptions) / teamTotal.TotalReceptions : 0,
                                RecYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.Yards) / teamTotal.TotalRecYds : 0,
                                RecTDShare = teamTotal.TotalRecTd > 0 ? stats.Sum(s => s.TD) / teamTotal.TotalRecTd : 0
                            });
                        }
                    }
                }
            }
            return share.Where(s => s.TotalFantasy > 0).OrderByDescending(s => s.FantasyShare).ToList();
        }

        public async Task<List<TeamTotals>> GetTeamTotals()
        {
            if (RetrieveFromCache().Any())
            {
                return RetrieveFromCache();
            }
            else
            {
                List<TeamTotals> totals = new();
                var teams = await _playersService.GetAllTeams();
                var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
                foreach (var team in teams)
                {
                    var totalFantasyQB = 0.0;
                    var totalFantasyRB = 0.0;
                    var totalFantasyWR = 0.0;
                    var totalFantasyTE = 0.0;
                    var totalRushAtt = 0.0;
                    var totalRushYd = 0.0;
                    var totalRushTd = 0.0;
                    var totalTargets = 0.0;
                    var totalReceptions = 0.0;
                    var totalRecYds = 0.0;
                    var totalRecTd = 0.0;

                    var players = await _playersService.GetPlayersByTeam(team.Team);
                    foreach (var p in players)
                    {
                        var player = await _playersService.GetPlayer(p.PlayerId);
                        _ = Enum.TryParse(player.Position, out PositionEnum position);
                        var fantasy = await _fantasyService.GetWeeklyFantasy(player.PlayerId);
                        if (position == PositionEnum.QB)
                        {
                            var stats = await _statisticsService.GetWeeklyDataQB(player.PlayerId);
                            totalFantasyQB += fantasy.Sum(f => f.FantasyPoints);
                            totalRushAtt += stats.Sum(s => s.RushingAttempts);
                            totalRushYd += stats.Sum(s => s.RushingYards);
                            totalRushTd += stats.Sum(s => s.RushingTD);
                        }
                        else if (position == PositionEnum.RB)
                        {
                            var stats = await _statisticsService.GetWeeklyDataRB(p.PlayerId);
                            totalFantasyRB += fantasy.Sum(f => f.FantasyPoints);
                            totalRushAtt += stats.Sum(s => s.RushingAtt);
                            totalRushYd += stats.Sum(s => s.RushingYds);
                            totalRushTd += stats.Sum(s => s.RushingTD);
                            totalTargets += stats.Sum(s => s.Targets);
                            totalReceptions += stats.Sum(s => s.Receptions);
                            totalRecYds += stats.Sum(s => s.Yards);
                            totalRecTd += stats.Sum(s => s.ReceivingTD);
                        }
                        else if (position == PositionEnum.WR)
                        {
                            var stats = await _statisticsService.GetWeeklyDataWR(p.PlayerId);
                            totalFantasyWR += fantasy.Sum(f => f.FantasyPoints);
                            totalRushAtt += stats.Sum(s => s.RushingAtt);
                            totalRushYd += stats.Sum(s => s.RushingYds);
                            totalRushTd += stats.Sum(s => s.RushingTD);
                            totalTargets += stats.Sum(s => s.Targets);
                            totalReceptions += stats.Sum(s => s.Receptions);
                            totalRecYds += stats.Sum(s => s.Yards);
                            totalRecTd += stats.Sum(s => s.TD);
                        }
                        else if (position == PositionEnum.TE)
                        {
                            var stats = await _statisticsService.GetWeeklyDataWR(p.PlayerId);
                            totalFantasyTE += fantasy.Sum(f => f.FantasyPoints);
                            totalRushAtt += stats.Sum(s => s.RushingAtt);
                            totalRushYd += stats.Sum(s => s.RushingYds);
                            totalRushTd += stats.Sum(s => s.RushingTD);
                            totalTargets += stats.Sum(s => s.Targets);
                            totalReceptions += stats.Sum(s => s.Receptions);
                            totalRecYds += stats.Sum(s => s.Yards);
                            totalRecTd += stats.Sum(s => s.TD);
                        }
                    }
                    totals.Add(new TeamTotals
                    {
                        Team = team,
                        Games = currentWeek - 1,
                        TotalFantasy = Math.Round(totalFantasyQB + totalFantasyRB + totalFantasyWR + totalFantasyTE, 2),
                        TotalFantasyQB = Math.Round(totalFantasyQB, 2),
                        TotalFantasyRB = Math.Round(totalFantasyRB, 2),
                        TotalFantasyWR = Math.Round(totalFantasyWR, 2),
                        TotalFantasyTE = Math.Round(totalFantasyTE, 2),
                        TotalRushAtt = Math.Round(totalRushAtt, 2),
                        TotalRushYd = Math.Round(totalRushYd, 2),
                        TotalRushTd = Math.Round(totalRushTd, 2),
                        TotalTargets = Math.Round(totalTargets, 2),
                        TotalReceptions = Math.Round(totalReceptions, 2),
                        TotalRecYds = Math.Round(totalRecYds, 2),
                        TotalRecTd = Math.Round(totalRecTd, 2)
                    });
                }
                var teamTotals = totals.OrderByDescending(t => t.TotalFantasy).ToList();
                _cache.Set("TeamTotals", teamTotals);
                return teamTotals;
            }
        }

        private List<TeamTotals> RetrieveFromCache() => _cache.TryGetValue("TeamTotals", out List<TeamTotals> totals) ? totals : Enumerable.Empty<TeamTotals>().ToList();
    }
}
