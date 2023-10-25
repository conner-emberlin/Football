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
                                                                            BoomCount = (double)wf.FantasyPoints.Where(fp => fp > _settingsService.GetBoomSetting(position)).Count(), 
                                                                            BustCount = (double)wf.FantasyPoints.Where(fp => fp < _settingsService.GetBustSetting(position)).Count(), 
                                                                            WeekCount = (double)wf.FantasyPoints.Count})                                                                        
                                                       .Select(bb => new BoomBust
                                                                {
                                                                     Season = _season.CurrentSeason,
                                                                     Player = bb.Player,
                                                                     BoomPercentage = bb.WeekCount > 0 ? Math.Round((bb.BoomCount/bb.WeekCount)*100) : 0,
                                                                     BustPercentage  = bb.WeekCount > 0 ? Math.Round(bb.BustCount/bb.WeekCount*100) : 0
                                                                }).ToList();
        }

        public async Task<List<BoomBustByWeek>> GetBoomBustsByWeek(int playerId)
        {
            List<BoomBustByWeek> boomBustsByWeek = new();
            var player = await _playersService.GetPlayer(playerId);
            var weeklyFantasy = await _fantasyDataService.GetWeeklyFantasy(player.PlayerId);
            if (Enum.TryParse(player.Position, out Position position))
            {
                var playerBoomBust = weeklyFantasy.Select(wf => new BoomBustByWeek
                {
                    Season = _season.CurrentSeason,
                    Player = player,
                    Week = wf.Week,
                    Boom = wf.FantasyPoints > _settingsService.GetBoomSetting(position),
                    Bust = wf.FantasyPoints < _settingsService.GetBustSetting(position),
                    FantasyPoints = wf.FantasyPoints
                });
                foreach (var wbb in playerBoomBust)
                {
                    boomBustsByWeek.Add(wbb);
                }
            }
            return boomBustsByWeek.OrderBy(b => b.Week).ToList();
        }

    }
}
