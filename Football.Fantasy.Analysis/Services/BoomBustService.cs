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
        private readonly BoomBustSettings _settings;
        private readonly IPlayersService _playersService;
        private readonly IFantasyDataService _fantasyDataService;

        public BoomBustService(IOptionsMonitor<Season> season, IOptionsMonitor<BoomBustSettings> settings, 
            IPlayersService playersService, IFantasyDataService fantasyDataService)
        {
            _season = season.CurrentValue;
            _settings = settings.CurrentValue;
            _playersService = playersService;
            _fantasyDataService = fantasyDataService;
        }
       public async Task<List<BoomBust>> GetBoomBusts(PositionEnum position)
        {
            var weeklyFantasy = await _fantasyDataService.GetWeeklyFantasy(position);
            var playersByPosition = await _playersService.GetPlayersByPosition(position);
            return  weeklyFantasy.Join(playersByPosition, wf => wf.PlayerId, p => p.PlayerId, 
                                       (wf, p) => new { WeeklyFantasy = wf, Player = p })
                                                       .GroupBy(wfp => wfp.Player, 
                                                                wfp => wfp.WeeklyFantasy.FantasyPoints, 
                                                                (key, f) => new {Player = key, FantasyPoints = f.ToList()})
                                                       .Select(wf => new { 
                                                                            wf.Player, 
                                                                            BoomCount = (double)wf.FantasyPoints.Where(fp => fp > GetBoomSetting(position)).Count(), 
                                                                            BustCount = (double)wf.FantasyPoints.Where(fp => fp < GetBustSetting(position)).Count(), 
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
            if (Enum.TryParse(player.Position, out PositionEnum position))
            {
                var playerBoomBust = weeklyFantasy.Select(wf => new BoomBustByWeek
                {
                    Season = _season.CurrentSeason,
                    Player = player,
                    Week = wf.Week,
                    Boom = wf.FantasyPoints > GetBoomSetting(position),
                    Bust = wf.FantasyPoints < GetBustSetting(position),
                    FantasyPoints = wf.FantasyPoints
                });
                foreach (var wbb in playerBoomBust)
                {
                    boomBustsByWeek.Add(wbb);
                }
            }
            return boomBustsByWeek.OrderBy(b => b.Week).ToList();
        }
        private double GetBoomSetting(PositionEnum position)
        {
            return position switch
            {
                PositionEnum.QB => _settings.QBBoom,
                PositionEnum.RB => _settings.RBBoom,
                PositionEnum.WR => _settings.WRBoom,
                PositionEnum.TE => _settings.TEBoom,
                _ => 0
            };
        }
        private double GetBustSetting(PositionEnum position)
        {
            return position switch
            {
                PositionEnum.QB => _settings.QBBust,
                PositionEnum.RB => _settings.RBBust,
                PositionEnum.WR => _settings.WRBust,
                PositionEnum.TE => _settings.TEBust,
                _ => 0
            };
        }



    }
}
