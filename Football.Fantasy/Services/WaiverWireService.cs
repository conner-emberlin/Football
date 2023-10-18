using Football.Models;
using Football.Data.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;
using Football.Enums;

namespace Football.Fantasy.Services
{
    public class WaiverWireService : IWaiverWireService
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IFantasyDataService _fantasyService;
        private readonly IPlayersService _playersService;
        private readonly WaiverWireSettings _settings;

        public WaiverWireService(IStatisticsService statisticsService, IFantasyDataService fantasyService, 
            IOptionsMonitor<WaiverWireSettings> settings, IPlayersService playersService)
        {
            _statisticsService = statisticsService;
            _fantasyService = fantasyService;
            _settings = settings.CurrentValue;
            _playersService = playersService;
        }

        public async Task<List<WeeklyRosterPercent>> GetWaiverWireCandidates(int season, int week)
        {
            List<WeeklyRosterPercent> candidates = new();
            var rosterPercentages = (await _statisticsService.GetWeeklyRosterPercentages(season, week - 1)).Where(r => r.RosterPercent < _settings.RostershipMinimum).ToList();
            foreach (var rp in rosterPercentages)
            {
                var rpPosition = (await _playersService.GetPlayer(rp.PlayerId)).Position;
                if (Enum.TryParse(rpPosition, out PositionEnum pos))
                {
                    //MAKE SURE THEY DID WELL RECENT WEEK
                    var weeklyFantasy = await _fantasyService.GetWeeklyFantasy(rp.PlayerId);
                    var goodWeekCount = weeklyFantasy.Where(w => w.FantasyPoints > GoodWeek(pos)).Count();
                    if(goodWeekCount > _settings.GoodWeekMinimum)
                    {
                        candidates.Add(rp);
                    }
                    else if (pos == PositionEnum.RB)
                    {
                        //is there an injury to the starting runningback
                    }
                }

            }
            return candidates.OrderBy(c => c.RosterPercent).ToList();
        }

        private double GoodWeek(PositionEnum position)
        {
            return position switch
            {
                PositionEnum.QB => _settings.GoodWeekFantasyPointsQB,
                PositionEnum.RB => _settings.GoodWeekFantasyPointsRB,
                PositionEnum.WR => _settings.GoodWeekFantasyPointsWR,
                PositionEnum.TE => _settings.GoodWeekFantasyPointsTE,
                _ => 0

            };
        }
    }
}
