﻿using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Football.Fantasy.Services
{
    public class MarketShareService(IPlayersService playersService, IStatisticsService statisticsService, IFantasyDataService fantasyService, ITeamsService teamsService,
        IOptionsMonitor<Season> season, IMemoryCache cache, ISettingsService settingsService) : IMarketShareService
    {
        private readonly Season _season = season.CurrentValue;

        public async Task<List<TargetShare>> GetTargetShares()
        {
            if (settingsService.GetFromCache<TargetShare>(Cache.TargetShares, out var cachedShares))
                return cachedShares;
            else
            {
                var teams = await teamsService.GetAllTeams();
                var playersDictionary = (await playersService.GetAllPlayers()).Where(p => p.Active == 1).ToDictionary(P => P.PlayerId);
                List<TargetShare> shares = [];
                foreach (var team in teams) shares.Add(await GetTargetShare(team, playersDictionary));
                var orderedShares = shares.OrderBy(s => s.Team.TeamDescription).ToList();
                cache.Set(Cache.TargetShares.ToString(), orderedShares);
                return orderedShares;
            }
        }
        public async Task<List<MarketShare>> GetMarketShare(Position position)
        {
            if (cache.TryGetValue(Cache.MarketShare.ToString() + position.ToString(), out List<MarketShare>? marketShares) && marketShares != null) 
                return marketShares;

            List<MarketShare> shares = [];
            var players = await playersService.GetPlayersByPosition(position, true);
            var teamTotals = (await GetTeamTotals()).ToDictionary(t => t.Team.TeamId);
            var playerTeams = (await teamsService.GetPlayerTeams(_season.CurrentSeason, players.Select(p => p.PlayerId))).ToDictionary(p => p.PlayerId);
            foreach (var player in players)
            {
                if (playerTeams.TryGetValue(player.PlayerId, out var team))
                {
                    shares.Add(await GetMarketShare(position, player, team, teamTotals[team.TeamId], await fantasyService.GetWeeklyFantasy(player.PlayerId, team.Team)));
                }                               
            }
            var mShares = shares.Where(s => s.TotalFantasy > 0).OrderByDescending(s => s.FantasyShare);
            cache.Set(Cache.MarketShare.ToString() + position.ToString(), mShares);
            return [.. mShares];
        }
        public async Task<List<TeamTotals>> GetTeamTotals()
        {
            if (settingsService.GetFromCache<TeamTotals>(Cache.TeamTotals, out var cachedTotals))
                return cachedTotals;
            else
            {
                List<TeamTotals> totals = [];
                var teams = await teamsService.GetAllTeams();
                foreach (var team in teams) totals.Add(await GetTeamTotals(team.TeamId));
                var teamTotals = totals.OrderByDescending(t => t.TotalFantasy).ToList();
                cache.Set(Cache.TeamTotals.ToString(), teamTotals);
                return teamTotals;
            }
        }

        public async Task<TeamTotals> GetTeamTotals(int teamId)
        {
            var team = await teamsService.GetTeam(teamId);
            var players = await teamsService.GetPlayersByTeam(team.Team);
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var schedule = (await teamsService.GetTeamGames(team.TeamId)).Where(t => t.Week < currentWeek && t.OpposingTeamId > 0);

            var total = new TeamTotals
            {
                Team = team,
                Games = schedule.Count()
            };

            foreach (var p in players)
            {
                var player = await playersService.GetPlayer(p.PlayerId);
                _ = Enum.TryParse(player.Position, out Position position);
                var fantasy = await fantasyService.GetWeeklyFantasy(player.PlayerId, team.Team);
                if (fantasy.Count > 0) total = await AccumulateTotals(position, total, p.PlayerId, team.Team, fantasy);
            }
            return total;
        }

        private async Task<TargetShare> GetTargetShare(TeamMap teamMap, Dictionary<int, Player> playersDictionary)
        {
            var teamPlayers = await teamsService.GetPlayersByTeam(teamMap.Team);

            var totalAttempts = 0.0;
            var totalCompletions = 0.0;
            var RBTargets = 0.0;
            var RBComps = 0.0;
            var WRTargets = 0.0;
            var WRComps = 0.0;
            var TETargets = 0.0;
            var TEComps = 0.0;

            foreach (var p in teamPlayers)
            {
                if (!(playersDictionary.TryGetValue(p.PlayerId, out var player) && Enum.TryParse<Position>(player.Position, out var position)))
                    continue;

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

        private async Task<MarketShare> GetMarketShare(Position position, Player player, PlayerTeam playerTeam, TeamTotals teamTotal, List<WeeklyFantasy> fantasy)
        {
            return position switch 
            { 
                Position.RB => await RBMarketShare(player, playerTeam, teamTotal, fantasy),
                Position.WR => await WRMarketShare(player, playerTeam, teamTotal, fantasy),
                Position.TE => await TEMarketShare(player, playerTeam, teamTotal, fantasy),
                _ => throw new NotImplementedException()
            };
        }

        private async Task<MarketShare> RBMarketShare(Player player, PlayerTeam playerTeam, TeamTotals teamTotal, List<WeeklyFantasy> fantasy)
        {
            var stats = await statisticsService.GetWeeklyData<WeeklyDataRB>(Position.RB, player.PlayerId, playerTeam.Team);
            return new MarketShare
            {
                Player = player,
                PlayerTeam = playerTeam,
                Games = fantasy.Count,
                TotalFantasy = fantasy.Sum(f => f.FantasyPoints),
                FantasyShare = teamTotal.TotalFantasyRB > 0 ? fantasy.Sum(f => f.FantasyPoints) / teamTotal.TotalFantasyRB : 0,
                RushAttShare = teamTotal.TotalRushAtt > 0 ? stats.Sum(s => s.RushingAtt) / teamTotal.TotalRushAtt : 0,
                RushYdShare = teamTotal.TotalRushYd > 0 ? stats.Sum(s => s.RushingYds) / teamTotal.TotalRushYd : 0,
                RushTDShare = teamTotal.TotalRushTd > 0 ? stats.Sum(s => s.RushingTD) / teamTotal.TotalRushTd : 0,
                TargetShare = teamTotal.TotalTargets > 0 ? stats.Sum(s => s.Targets) / teamTotal.TotalTargets : 0,
                ReceptionShare = teamTotal.TotalReceptions > 0 ? stats.Sum(s => s.Receptions) / teamTotal.TotalReceptions : 0,
                RecYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.Yards) / teamTotal.TotalRecYds : 0,
                RecTDShare = teamTotal.TotalRecTd > 0 ? stats.Sum(s => s.ReceivingTD) / teamTotal.TotalRecTd : 0
            };
        }

        private async Task<MarketShare> WRMarketShare(Player player, PlayerTeam playerTeam, TeamTotals teamTotal, List<WeeklyFantasy> fantasy)
        {
            var stats = await statisticsService.GetWeeklyData<WeeklyDataWR>(Position.WR, player.PlayerId, playerTeam.Team);
            return new MarketShare
            {
                Player = player,
                PlayerTeam = playerTeam,
                Games = fantasy.Count,
                TotalFantasy = fantasy.Sum(f => f.FantasyPoints),
                FantasyShare = teamTotal.TotalFantasyWR > 0 ? fantasy.Sum(f => f.FantasyPoints) / teamTotal.TotalFantasyWR : 0,
                RushAttShare = teamTotal.TotalRushAtt > 0 ? stats.Sum(s => s.RushingAtt) / teamTotal.TotalRushAtt : 0,
                RushYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.RushingYds) / teamTotal.TotalRecYds : 0,
                RushTDShare = teamTotal.TotalRushTd > 0 ? stats.Sum(s => s.RushingTD) / teamTotal.TotalRushTd : 0,
                TargetShare = teamTotal.TotalTargets > 0 ? stats.Sum(s => s.Targets) / teamTotal.TotalTargets : 0,
                ReceptionShare = teamTotal.TotalReceptions > 0 ? stats.Sum(s => s.Receptions) / teamTotal.TotalReceptions : 0,
                RecYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.Yards) / teamTotal.TotalRecYds : 0,
                RecTDShare = teamTotal.TotalRecTd > 0 ? stats.Sum(s => s.TD) / teamTotal.TotalRecTd : 0
            };
        }

        private async Task<MarketShare> TEMarketShare(Player player, PlayerTeam playerTeam, TeamTotals teamTotal, List<WeeklyFantasy> fantasy)
        {
            var stats = await statisticsService.GetWeeklyData<WeeklyDataTE>(Position.TE, player.PlayerId, playerTeam.Team);
            return new MarketShare
            {
                Player = player,
                PlayerTeam = playerTeam,
                Games = fantasy.Count,
                TotalFantasy = fantasy.Sum(f => f.FantasyPoints),
                FantasyShare = teamTotal.TotalFantasyTE > 0 ? fantasy.Sum(f => f.FantasyPoints) / teamTotal.TotalFantasyTE : 0,
                RushAttShare = teamTotal.TotalRushAtt > 0 ? stats.Sum(s => s.RushingAtt) / teamTotal.TotalRushAtt : 0,
                RushYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.RushingYds) / teamTotal.TotalRecYds : 0,
                RushTDShare = teamTotal.TotalRushTd > 0 ? stats.Sum(s => s.RushingTD) / teamTotal.TotalRushTd : 0,
                TargetShare = teamTotal.TotalTargets > 0 ? stats.Sum(s => s.Targets) / teamTotal.TotalTargets : 0,
                ReceptionShare = teamTotal.TotalReceptions > 0 ? stats.Sum(s => s.Receptions) / teamTotal.TotalReceptions : 0,
                RecYdShare = teamTotal.TotalRecYds > 0 ? stats.Sum(s => s.Yards) / teamTotal.TotalRecYds : 0,
                RecTDShare = teamTotal.TotalRecTd > 0 ? stats.Sum(s => s.TD) / teamTotal.TotalRecTd : 0
            };
        }

        private async Task<TeamTotals> AccumulateTotals(Position position, TeamTotals total, int playerId, string team, List<WeeklyFantasy> fantasy)
        {
            return position switch
            {
                Position.QB => await AccumulateQBTotals(total, playerId, team, fantasy),
                Position.RB => await AccumulateRBTotals(total, playerId, team, fantasy),
                Position.WR => await AccumulateWRTotals(total, playerId, team, fantasy),
                Position.TE => await AccumulateTETotals(total, playerId, team, fantasy),
                _ => total
            };
        }

        private async Task<TeamTotals> AccumulateQBTotals(TeamTotals total, int playerId, string team, List<WeeklyFantasy> fantasy)
        {
            var stats = await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, playerId, team);
            total.TotalFantasyQB += fantasy.Sum(f => f.FantasyPoints);
            total.TotalRushAtt += stats.Sum(s => s.RushingAttempts);
            total.TotalRushYd += stats.Sum(s => s.RushingYards);
            total.TotalRushTd += stats.Sum(s => s.RushingTD);
            return total;
        }

        private async Task<TeamTotals> AccumulateRBTotals(TeamTotals total, int playerId, string team, List<WeeklyFantasy> fantasy)
        {
            var stats = await statisticsService.GetWeeklyData<WeeklyDataRB>(Position.RB, playerId, team);
            total.TotalFantasyRB += fantasy.Sum(f => f.FantasyPoints);
            total.TotalRushAtt += stats.Sum(s => s.RushingAtt);
            total.TotalRushYd += stats.Sum(s => s.RushingYds);
            total.TotalRushTd += stats.Sum(s => s.RushingTD);
            total.TotalTargets += stats.Sum(s => s.Targets);
            total.TotalReceptions += stats.Sum(s => s.Receptions);
            total.TotalRecYds += stats.Sum(s => s.Yards);
            total.TotalRecTd += stats.Sum(s => s.ReceivingTD);
            return total;
        }

        private async Task<TeamTotals> AccumulateWRTotals(TeamTotals total, int playerId, string team, List<WeeklyFantasy> fantasy)
        {
            var stats = await statisticsService.GetWeeklyData<WeeklyDataWR>(Position.WR, playerId, team);
            total.TotalFantasyWR += fantasy.Sum(f => f.FantasyPoints);
            total.TotalRushAtt += stats.Sum(s => s.RushingAtt);
            total.TotalRushYd += stats.Sum(s => s.RushingYds);
            total.TotalRushTd += stats.Sum(s => s.RushingTD);
            total.TotalTargets += stats.Sum(s => s.Targets);
            total.TotalReceptions += stats.Sum(s => s.Receptions);
            total.TotalRecYds += stats.Sum(s => s.Yards);
            total.TotalRecTd += stats.Sum(s => s.TD);
            return total;
        }

        private async Task<TeamTotals> AccumulateTETotals(TeamTotals total, int playerId, string team, List<WeeklyFantasy> fantasy)
        {
            var stats = await statisticsService.GetWeeklyData<WeeklyDataTE>(Position.TE, playerId, team);
            total.TotalFantasyTE += fantasy.Sum(f => f.FantasyPoints);
            total.TotalRushAtt += stats.Sum(s => s.RushingAtt);
            total.TotalRushYd += stats.Sum(s => s.RushingYds);
            total.TotalRushTd += stats.Sum(s => s.RushingTD);
            total.TotalTargets += stats.Sum(s => s.Targets);
            total.TotalReceptions += stats.Sum(s => s.Receptions);
            total.TotalRecYds += stats.Sum(s => s.Yards);
            total.TotalRecTd += stats.Sum(s => s.TD);
            return total;
        }

    }
}
