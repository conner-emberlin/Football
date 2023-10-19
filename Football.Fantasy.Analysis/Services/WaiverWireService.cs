using Football.Models;
using Football.Data.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Football.Statistics.Interfaces;
using Football.Fantasy.Analysis.Interfaces;
using Microsoft.Extensions.Options;
using Football.Enums;

namespace Football.Fantasy.Analysis.Services
{
    public class WaiverWireService : IWaiverWireService
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IFantasyDataService _fantasyService;
        private readonly IPlayersService _playersService;
        private readonly WaiverWireSettings _settings;
        private readonly Season _season;

        public WaiverWireService(IStatisticsService statisticsService, IFantasyDataService fantasyService, 
            IOptionsMonitor<WaiverWireSettings> settings, IPlayersService playersService, IOptionsMonitor<Season> season)
        {
            _statisticsService = statisticsService;
            _fantasyService = fantasyService;
            _settings = settings.CurrentValue;
            _playersService = playersService;
            _season = season.CurrentValue;
        }

        public async Task<List<WeeklyRosterPercent>> GetWaiverWireCandidates(int season, int week)
        {
            List<WeeklyRosterPercent> candidates = new();
            var rosterPercentages = (await _statisticsService.GetWeeklyRosterPercentages(season, week - 1)).Where(r => r.RosterPercent < _settings.RostershipMax).ToList();
            foreach (var rp in rosterPercentages)
            {
                var rpPosition = (await _playersService.GetPlayer(rp.PlayerId)).Position;
                if (Enum.TryParse(rpPosition, out PositionEnum pos))
                {
                    var weeklyFantasy = await _fantasyService.GetWeeklyFantasy(rp.PlayerId);
                    var goodWeekCount = weeklyFantasy.Where(w => w.FantasyPoints > GoodWeek(pos)).Count();
                    var recentWeekFantasy = weeklyFantasy.Where(wf => wf.Week == week - 1).FirstOrDefault();            
                    if(goodWeekCount > _settings.GoodWeekMinimum && recentWeekFantasy != null)
                    {
                        if (recentWeekFantasy.FantasyPoints > GoodWeek(pos))
                        {
                            candidates.Add(rp);
                        }                               
                    }
                    else if (pos == PositionEnum.RB)
                    {
                        if (await InjuryToLeadRB(rp.PlayerId, week))
                        {
                            candidates.Add(rp);
                        }
                    }
                }
            }
            return candidates.OrderByDescending(c => c.RosterPercent).ToList();
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
        private async Task<bool> InjuryToLeadRB(int playerId, int week)
        {                       
            var playerTeam = await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId);            
            if (playerTeam != null)
            {

                var activeInjuries = await _playersService.GetActiveInSeasonInjuries(_season.CurrentSeason);
                var otherPlayersOnTeam = (await _playersService.GetPlayersByTeam(playerTeam.Team)).Where(p => p.PlayerId != playerId);
                var injuredPlayersOnTeam = activeInjuries.Join(otherPlayersOnTeam, ai => ai.PlayerId, op => op.PlayerId, (ai, op) => new { InSeasonInjury = ai, PlayerTeam = op })
                                                         .Where(ipot => ipot.InSeasonInjury.InjuryStartWeek == week - 1).ToList();
                if (injuredPlayersOnTeam.Any())
                {
                    foreach (var injuredPlayer in injuredPlayersOnTeam)
                    {
                        var position = (await _playersService.GetPlayer(injuredPlayer.PlayerTeam.PlayerId)).Position;
                        if (position == PositionEnum.RB.ToString())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }    
    }
}
