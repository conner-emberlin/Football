using Football.Enums;
using Football.Models;
using Football.Fantasy.Analysis.Models;
using Football.Fantasy.Analysis.Interfaces;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Microsoft.Extensions.Options;


namespace Football.Fantasy.Analysis.Services
{
    public class BoomBustService : IBoomBustService
    {
        private readonly Season _season;
        private readonly IPlayersService _playersService;
        private readonly IFantasyDataService _fantasyDataService;
        private readonly ISettingsService _settingsService;

        public BoomBustService(IOptionsMonitor<Season> season, 
            IPlayersService playersService, IFantasyDataService fantasyDataService, ISettingsService settingsService)
        {
            _season = season.CurrentValue;
            _playersService = playersService;
            _fantasyDataService = fantasyDataService;
            _settingsService = settingsService;
        }
        public async Task<List<BoomBust>> GetBoomBusts(Position position)
        {
            var weeklyFantasy = await _fantasyDataService.GetWeeklyFantasy(position);
            var playersByPosition = await _playersService.GetPlayersByPosition(position);
            return weeklyFantasy.Join(playersByPosition, wf => wf.PlayerId, p => p.PlayerId, 
                                       (wf, p) => new { WeeklyFantasy = wf, Player = p })
                                                       .GroupBy(wfp => wfp.Player, 
                                                                wfp => wfp.WeeklyFantasy.FantasyPoints, 
                                                                (key, f) => new {Player = key, FantasyPoints = f.ToList()})
                                                       .Select(wf => new { 
                                                                            wf.Player, 
                                                                            BoomCount = (double)wf.FantasyPoints.Count(fp => fp > _settingsService.GetBoomSetting(position)), 
                                                                            BustCount = (double)wf.FantasyPoints.Count(fp => fp < _settingsService.GetBustSetting(position)), 
                                                                            WeekCount = (double)wf.FantasyPoints.Count})                                                                        
                                                       .Select(bb => new BoomBust
                                                                {
                                                                     Season = _season.CurrentSeason,
                                                                     Player = bb.Player,
                                                                     BoomPercentage = bb.WeekCount > 0 ? Math.Round((bb.BoomCount/bb.WeekCount)*100) : 0,
                                                                     BustPercentage  = bb.WeekCount > 0 ? Math.Round(bb.BustCount/bb.WeekCount*100) : 0
                                                                }).ToList();
        }

        public async Task<List<FantasyPerformance>> GetFantasyPerformances(Position position)
        {
            List<FantasyPerformance> fantasyPerformances = new();
            var players = await _playersService.GetPlayersByPosition(position);
            foreach (var player in players)
            {
                var weeklyFantasy = await _fantasyDataService.GetWeeklyFantasy(player.PlayerId);
                if (weeklyFantasy.Any())
                {
                    var variance = CalculateVariance(weeklyFantasy.Select(w => w.FantasyPoints));
                    fantasyPerformances.Add(new FantasyPerformance
                    {
                        PlayerId = player.PlayerId,
                        Name = player.Name,
                        Position = player.Position,
                        Season = _season.CurrentSeason,
                        AvgFantasy = weeklyFantasy.Average(w => w.FantasyPoints),
                        MinFantasy = weeklyFantasy.Min(w => w.FantasyPoints),
                        MaxFantasy = weeklyFantasy.Max(w => w.FantasyPoints),
                        Variance = variance,
                        StdDev = Math.Sqrt(variance)
                    }) ;
                }
            }
            return fantasyPerformances.Where(f => f.AvgFantasy > 0).OrderByDescending(f => f.AvgFantasy).ToList();
        }
        public async Task<List<BoomBustByWeek>> GetBoomBustsByWeek(int playerId)
        {
            var player = await _playersService.GetPlayer(playerId);
            var weeklyFantasy = await _fantasyDataService.GetWeeklyFantasy(player.PlayerId);
            if (Enum.TryParse(player.Position, out Position position))
            {
                 return weeklyFantasy.Select(wf => new BoomBustByWeek
                 {
                    Season = _season.CurrentSeason,
                    Player = player,
                    Week = wf.Week,
                    Boom = wf.FantasyPoints > _settingsService.GetBoomSetting(position),
                    Bust = wf.FantasyPoints < _settingsService.GetBustSetting(position),
                    FantasyPoints = wf.FantasyPoints
                 }).OrderBy(b => b.Week).ToList();
            }
            else return Enumerable.Empty<BoomBustByWeek>().ToList();
        }

        private static double CalculateVariance(IEnumerable<double> fantasyPoints)
        {
            var mean = fantasyPoints.Average();
            var count = fantasyPoints.Count();
            var sumOfSquares = 0.0;
            foreach (var fp in fantasyPoints)
            {
                sumOfSquares += Math.Pow(fp - mean, 2);
            }

            return sumOfSquares / count;
        }
    }
}
