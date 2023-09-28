using Football.Fantasy.Interfaces;
using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Data.Models;
using Microsoft.Extensions.Options;
using Football.Fantasy.Models;

namespace Football.Fantasy.Services
{
    public class MarketShareService : IMarketShareService
    {
        private readonly IPlayersService _playersService;
        private readonly IStatisticsService _statisticsService;
        private readonly IFantasyDataService _fantasyService;
        private readonly Season _season;

        public MarketShareService(IPlayersService playersService, IStatisticsService statisticsService, IFantasyDataService fantasyService,
            IOptionsMonitor<Season> season)
        {
            _playersService = playersService;
            _statisticsService = statisticsService;
            _fantasyService = fantasyService;
            _season = season.CurrentValue;
        }
        public async Task<List<TeamTotals>> GetTeamTotals()
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
                    TotalFantasy = totalFantasyQB + totalFantasyRB + totalFantasyWR + totalFantasyTE,
                    TotalFantasyQB = totalFantasyQB,
                    TotalFantasyRB = totalFantasyRB,
                    TotalFantasyWR = totalFantasyWR,
                    TotalFantasyTE = totalFantasyTE,
                    TotalRushAtt = totalRushAtt,
                    TotalRushYd = totalRushYd,
                    TotalRushTd = totalRushTd,
                    TotalTargets = totalTargets,
                    TotalReceptions = totalReceptions,
                    TotalRecYds = totalRecYds,
                    TotalRecTd = totalRecTd
                });
            }
            return totals.OrderByDescending(t => t.TotalFantasy).ToList();
        }
    }
}
