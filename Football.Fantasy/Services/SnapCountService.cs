using Football.Enums;
using Football.Players.Models;
using Football.Players.Interfaces;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;



namespace Football.Fantasy.Services
{
    public class SnapCountService(IPlayersService playersService, IStatisticsService statisticsService, IFantasyDataService fantasyDataService) : ISnapCountService
    {
        public async Task<List<SnapCountAnalysis>> GetSnapCountAnalysis(Position position, int season)
        {
            var players = await playersService.GetPlayersByPosition(position);
            List<SnapCountAnalysis> snapCountAnalyses = [];
            foreach (var player in players)
            {
                var sca = await GetSnapCountAnalysis(player, season);
                if ( sca != null )
                    snapCountAnalyses.Add(sca);
            }
            return snapCountAnalyses;
        }

        private async Task<SnapCountAnalysis?> GetSnapCountAnalysis(Player player, int season)
        {
            var snapCounts = (await statisticsService.GetSnapCounts(player.PlayerId)).Where(s => s.Season == season);
            var totalSnaps = snapCounts.Select(s => s.Snaps).DefaultIfEmpty(0).Sum();
            if (snapCounts.Any() && totalSnaps > 0)
            {
                var rushAtts = await RushAttempts(player, season, snapCounts.Select(s => s.Week));
                var targets = await Targets(player, season, snapCounts.Select(s => s.Week));
                var passAtts = player.Position == Position.QB.ToString() ? await PassAttempts(player, season, snapCounts.Select(s => s.Week)) : 0;

                return new SnapCountAnalysis
                {
                    Player  = player,
                    Season = season,
                    TotalSnaps = totalSnaps,
                    SnapsPerGame = snapCounts.Average(s => s.Snaps),
                    FantasyPointsPerSnap = await FantasyPointsPerSnap(player, season, totalSnaps),
                    RushAttsPerSnap = rushAtts/totalSnaps,
                    TargetsPerSnap = targets/totalSnaps,
                    Utilization = (targets + rushAtts + passAtts)/totalSnaps
                };
            }
            else return null;            
        }

        private async Task<double> FantasyPointsPerSnap(Player player, int season, double totalSnaps)
        {
            var weeklyFantasy = (await fantasyDataService.GetWeeklyFantasy(player.PlayerId)).Where(f => f.Season == season);
            if (weeklyFantasy.Any())
                return weeklyFantasy.Sum(w => w.FantasyPoints) / totalSnaps;
            return 0;
        }

        private async Task<double> RushAttempts(Player player, int season, IEnumerable<int> weeks)
        {
            if (Enum.TryParse(player.Position, out Position position))
            {
                var rushAtts = position switch
                {
                    Position.QB => (await statisticsService.GetWeeklyData<WeeklyDataQB>(position, player.PlayerId)).Where(w => w.Season == season && weeks.Contains(w.Week)).Select(w => w.RushingAttempts).DefaultIfEmpty(0).Sum(),
                    Position.RB => (await statisticsService.GetWeeklyData<WeeklyDataRB>(position, player.PlayerId)).Where(w => w.Season == season && weeks.Contains(w.Week)).Select(w => w.RushingAtt).DefaultIfEmpty(0).Sum(),
                    Position.WR => (await statisticsService.GetWeeklyData<WeeklyDataWR>(position, player.PlayerId)).Where(w => w.Season == season && weeks.Contains(w.Week)).Select(w => w.RushingAtt).DefaultIfEmpty(0).Sum(),
                    Position.TE => (await statisticsService.GetWeeklyData<WeeklyDataTE>(position, player.PlayerId)).Where(w => w.Season == season && weeks.Contains(w.Week)).Select(w => w.RushingAtt).DefaultIfEmpty(0).Sum(),
                    _ => 0
                };
                return rushAtts;
            }
            return 0;
        }

        private async Task<double> Targets(Player player, int season, IEnumerable<int> weeks)
        {
            if (Enum.TryParse(player.Position, out Position position))
            {
                var rushAtts = position switch
                {
                    Position.RB => (await statisticsService.GetWeeklyData<WeeklyDataRB>(position, player.PlayerId)).Where(w => w.Season == season && weeks.Contains(w.Week)).Select(w => w.Targets).DefaultIfEmpty(0).Sum(),
                    Position.WR => (await statisticsService.GetWeeklyData<WeeklyDataWR>(position, player.PlayerId)).Where(w => w.Season == season && weeks.Contains(w.Week)).Select(w => w.Targets).DefaultIfEmpty(0).Sum(),
                    Position.TE => (await statisticsService.GetWeeklyData<WeeklyDataTE>(position, player.PlayerId)).Where(w => w.Season == season && weeks.Contains(w.Week)).Select(w => w.Targets).DefaultIfEmpty(0).Sum(),
                    _ => 0
                };
                return rushAtts;
            }
            return 0;
        }

        private async Task<double> PassAttempts(Player player, int season, IEnumerable<int> weeks) => (await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, player.PlayerId)).Where(w => w.Season == season && weeks.Contains(w.Week)).Select(w => w.Attempts).DefaultIfEmpty(0).Sum();
    }
}
