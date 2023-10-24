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
        private readonly ISettingsService _settingsService;
        private readonly WaiverWireSettings _settings;
        private readonly Season _season;

        public WaiverWireService(IStatisticsService statisticsService, IFantasyDataService fantasyService, 
            ISettingsService settingsService, IPlayersService playersService, IOptionsMonitor<Season> season, IOptionsMonitor<WaiverWireSettings> settings)
        {
            _statisticsService = statisticsService;
            _fantasyService = fantasyService;
            _settingsService = settingsService;
            _playersService = playersService;
            _season = season.CurrentValue;
            _settings = settings.CurrentValue;
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
                    var goodWeekCount = weeklyFantasy.Where(w => w.FantasyPoints > _settingsService.GoodWeek(pos)).Count();
                    var recentWeekFantasy = weeklyFantasy.Where(wf => wf.Week == week - 1).FirstOrDefault();            
                    if(goodWeekCount > _settings.GoodWeekMinimum && recentWeekFantasy != null)
                    {
                        if (recentWeekFantasy.FantasyPoints > _settingsService.GoodWeek(pos))
                        {
                            candidates.Add(rp);
                        }                               
                    }
                    else if (pos == PositionEnum.RB)
                    {
                        if (await InjuryToLeadRB(rp.PlayerId))
                        {
                            candidates.Add(rp);
                        }
                    }
                }
            }
            candidates.ForEach(c => c.Week += 1);
            return candidates.OrderByDescending(c => c.RosterPercent).ToList();
        }
        private async Task<bool> InjuryToLeadRB(int playerId)
        {                       
            var playerTeam = await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId);            
            if (playerTeam != null)
            {

                var activeInjuries = await _playersService.GetActiveInSeasonInjuries(_season.CurrentSeason);
                var otherPlayersOnTeam = (await _playersService.GetPlayersByTeam(playerTeam.Team)).Where(p => p.PlayerId != playerId);
                var injuredPlayersOnTeam = activeInjuries.Join(otherPlayersOnTeam, ai => ai.PlayerId, op => op.PlayerId, (ai, op) => new { InSeasonInjury = ai, PlayerTeam = op });                                                        
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
