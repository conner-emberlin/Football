using Football.Enums;
using Football.Models;
using Football.Data.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Analysis.Models;
using Football.Fantasy.Analysis.Interfaces;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Statistics.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Football.Fantasy.Analysis.Services
{
    public class MarketShareService(IPlayersService playersService, IStatisticsService statisticsService, IFantasyDataService fantasyService,
        IOptionsMonitor<Season> season, IMemoryCache cache, ISettingsService settingsService) : IMarketShareService
    {
        private readonly Season _season = season.CurrentValue;

        public async Task<TargetShare> GetTargetShare(int playerId)
        {
            var team = await playersService.GetPlayerTeam(_season.CurrentSeason, playerId);
            if (team != null) return await GetTargetShare(await playersService.GetTeam(team.TeamId));
            return new();    
        }
        public async Task<List<TargetShare>> GetTargetShares()
        {
            if (settingsService.GetFromCache<TargetShare>(Cache.TargetShares, out var cachedShares))
                return cachedShares;
            else
            {
                var teams = await playersService.GetAllTeams();
                List<TargetShare> shares = [];
                foreach (var team in teams) shares.Add(await GetTargetShare(team));
                var orderedShares = shares.OrderBy(s => s.Team.TeamDescription).ToList();
                cache.Set(Cache.TargetShares.ToString(), orderedShares);
                return orderedShares;
            }
        }
        public async Task<List<MarketShare>> GetMarketShare(Position position)
        {
            List<MarketShare> share = [];
            var players = await playersService.GetPlayersByPosition(position);
            var teamTotals = await GetTeamTotals();
            foreach (var player in players)
            {
                var team = await playersService.GetPlayerTeam(_season.CurrentSeason, player.PlayerId);
                if (team != null) {
                    var fantasy = await fantasyService.GetWeeklyFantasy(player.PlayerId, team.Team);
                    var teamTotal = teamTotals.First(t => t.Team.Team == team.Team);
                    if (position == Position.RB)
                    {
                        var stats = await statisticsService.GetWeeklyData<WeeklyDataRB>(position, player.PlayerId, team.Team);
                        if (stats.Count > 0)
                        {
                            share.Add(new MarketShare
                            {
                                Player = player,
                                PlayerTeam = team,
                                Games = teamTotals.First().Games,
                                TotalFantasy = fantasy.Sum(f => f.FantasyPoints),
                                FantasyShare = teamTotal.TotalFantasyRB > 0 ? fantasy.Sum(f => f.FantasyPoints) / teamTotal.TotalFantasyRB : 0,
                                RushAttShare = teamTotal.TotalRushAtt > 0 ? stats.Sum(s => s.RushingAtt) / teamTotal.TotalRushAtt : 0,
                                RushYdShare = teamTotal.TotalRushYd > 0 ? stats.Sum(s => s.RushingYds) / teamTotal.TotalRushYd : 0,
                                RushTDShare = teamTotal.TotalRushTd > 0 ? stats.Sum(s => s.RushingTD) / teamTotal.TotalRushTd : 0,
                                TargetShare = teamTotal.TotalTargets > 0 ? stats.Sum(s => s.Targets) / teamTotal.TotalTargets : 0,
                                ReceptionShare = teamTotal.TotalReceptions > 0 ? stats.Sum(s => s.Receptions) / teamTotal.TotalReceptions : 0,
                                RecYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.Yards) / teamTotal.TotalRecYds : 0,
                                RecTDShare = teamTotal.TotalRecTd > 0 ? stats.Sum(s => s.ReceivingTD) / teamTotal.TotalRecTd : 0
                            });
                        }
                    }
                    else if (position == Position.WR)
                    {
                        var stats = await statisticsService.GetWeeklyData<WeeklyDataWR>(position, player.PlayerId, team.Team);
                        if (stats.Count > 0)
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
                    else if (position == Position.TE)
                    {
                        var stats = await statisticsService.GetWeeklyData<WeeklyDataTE>(position, player.PlayerId, team.Team);
                        if (stats.Count > 0)
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
            if (settingsService.GetFromCache<TeamTotals>(Cache.TeamTotals, out var cachedTotals))
                return cachedTotals;
            else
            {
                List<TeamTotals> totals = [];
                var teams = await playersService.GetAllTeams();
                foreach (var team in teams)
                    totals.Add(await GetTeamTotals(team.TeamId));
                var teamTotals = totals.OrderByDescending(t => t.TotalFantasy).ToList();
                cache.Set(Cache.TeamTotals.ToString(), teamTotals);
                return teamTotals;
            }
        }

        public async Task<TeamTotals> GetTeamTotals(int teamId)
        {
            var team = await playersService.GetTeam(teamId);
            var players = await playersService.GetPlayersByTeam(team.Team);
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);

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

            foreach (var p in players)
            {
                var player = await playersService.GetPlayer(p.PlayerId);
                _ = Enum.TryParse(player.Position, out Position position);
                var fantasy = await fantasyService.GetWeeklyFantasy(player.PlayerId, team.Team);
                if (position == Position.QB)
                {
                    var stats = await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, player.PlayerId, team.Team);
                    totalFantasyQB += fantasy.Sum(f => f.FantasyPoints);
                    totalRushAtt += stats.Sum(s => s.RushingAttempts);
                    totalRushYd += stats.Sum(s => s.RushingYards);
                    totalRushTd += stats.Sum(s => s.RushingTD);
                }
                else if (position == Position.RB)
                {
                    var stats = await statisticsService.GetWeeklyData<WeeklyDataRB>(position, p.PlayerId, team.Team);
                    totalFantasyRB += fantasy.Sum(f => f.FantasyPoints);
                    totalRushAtt += stats.Sum(s => s.RushingAtt);
                    totalRushYd += stats.Sum(s => s.RushingYds);
                    totalRushTd += stats.Sum(s => s.RushingTD);
                    totalTargets += stats.Sum(s => s.Targets);
                    totalReceptions += stats.Sum(s => s.Receptions);
                    totalRecYds += stats.Sum(s => s.Yards);
                    totalRecTd += stats.Sum(s => s.ReceivingTD);
                }
                else if (position == Position.WR)
                {
                    var stats = await statisticsService.GetWeeklyData<WeeklyDataWR>(position, p.PlayerId, team.Team);
                    totalFantasyWR += fantasy.Sum(f => f.FantasyPoints);
                    totalRushAtt += stats.Sum(s => s.RushingAtt);
                    totalRushYd += stats.Sum(s => s.RushingYds);
                    totalRushTd += stats.Sum(s => s.RushingTD);
                    totalTargets += stats.Sum(s => s.Targets);
                    totalReceptions += stats.Sum(s => s.Receptions);
                    totalRecYds += stats.Sum(s => s.Yards);
                    totalRecTd += stats.Sum(s => s.TD);
                }
                else if (position == Position.TE)
                {
                    var stats = await statisticsService.GetWeeklyData<WeeklyDataTE>(position, p.PlayerId, team.Team);
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
            return new TeamTotals
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
            };
        }

        private async Task<TargetShare> GetTargetShare(TeamMap teamMap)
        {
            var allPlayers = await playersService.GetPlayersByTeam(teamMap.Team);
            List<Player> players = [];

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
                _ = Enum.TryParse((await playersService.GetPlayer(p.PlayerId)).Position, out Position position);
                if (position == Position.QB)
                {
                    var stats = await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, p.PlayerId, teamMap.Team);
                    totalAttempts += stats.Sum(s => s.Attempts);
                    totalCompletions += stats.Sum(s => s.Completions);
                }
                else if (position == Position.RB)
                {
                    var stats = await statisticsService.GetWeeklyData<WeeklyDataRB>(position, p.PlayerId, teamMap.Team);
                    RBTargets += stats.Sum(s => s.Targets);
                    RBComps += stats.Sum(s => s.Receptions);
                }
                else if (position == Position.WR)
                {
                    var stats = await statisticsService.GetWeeklyData<WeeklyDataWR>(position, p.PlayerId, teamMap.Team);
                    WRTargets += stats.Sum(s => s.Targets);
                    WRComps += stats.Sum(s => s.Receptions);
                }
                else if (position == Position.TE)
                {
                    var stats = await statisticsService.GetWeeklyData<WeeklyDataTE>(position, p.PlayerId, teamMap.Team);
                    TETargets += stats.Sum(s => s.Targets);
                    TEComps += stats.Sum(s => s.Receptions);
                }
            }
            return new TargetShare
            {
                Team = teamMap,
                RBTargetShare = totalAttempts > 0 ? Math.Round(RBTargets / totalAttempts, 3) : 0,
                RBCompShare = totalCompletions > 0 ? Math.Round(RBComps / totalCompletions, 3) : 0,
                WRTargetShare = totalAttempts > 0 ? Math.Round(WRTargets / totalAttempts, 3) : 0,
                WRCompShare = totalCompletions > 0 ? Math.Round(WRComps / totalCompletions, 3) : 0,
                TETargetShare = totalAttempts > 0 ? Math.Round(TETargets / totalAttempts, 3) : 0,
                TECompShare = totalCompletions > 0 ? Math.Round(TEComps / totalCompletions, 3) : 0
            };
        }


    }
}
