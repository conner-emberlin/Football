using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Football.Statistics.Interfaces;
using Football.Fantasy.Analysis.Interfaces;
using Football.Fantasy.Analysis.Models;
using Football.Enums;
using Microsoft.Extensions.Options;

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

        public async Task<List<WaiverWireCandidate>> GetWaiverWireCandidates(int season, int week)
        {
            List<WaiverWireCandidate> candidates = [];
            if (week > _season.Weeks) 
                return candidates;

            var rosterPercentages = (await _statisticsService.GetWeeklyRosterPercentages(season, week - 1)).Where(r => r.RosterPercent < _settings.RostershipMax).ToList();
            foreach (var rp in rosterPercentages)
            {
                var player = await _playersService.GetPlayer(rp.PlayerId);
                if (Enum.TryParse(player.Position, out Position pos) && player.Active == 1)
                {
                    var weeklyFantasy = await _fantasyService.GetWeeklyFantasy(rp.PlayerId);
                    var goodWeekCount = weeklyFantasy.Count(w => w.FantasyPoints > _settingsService.GoodWeek(pos));
                    var recentWeekFantasy = weeklyFantasy.FirstOrDefault(wf => wf.Week == week - 1);            
                    if(goodWeekCount > _settings.GoodWeekMinimum && recentWeekFantasy != null)
                    {
                        if (recentWeekFantasy.FantasyPoints > _settingsService.GoodWeek(pos))
                        {
                            candidates.Add(new WaiverWireCandidate
                            {
                                Player = player,
                                PlayerTeam = await _playersService.GetPlayerTeam(_season.CurrentSeason, player.PlayerId),
                                Week = rp.Week + 1,
                                RosteredPercentage = rp.RosterPercent
                            });
                        }                               
                    }
                    else if (pos == Position.RB)
                    {
                        if (await InjuryToLeadRB(rp.PlayerId))
                        {
                            candidates.Add(new WaiverWireCandidate
                            {
                                Player = player,
                                PlayerTeam = await _playersService.GetPlayerTeam(_season.CurrentSeason, player.PlayerId),
                                Week = rp.Week + 1,
                                RosteredPercentage = rp.RosterPercent
                            });
                        }
                    }
                }
            }
            return candidates; 
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
                        if (position == Position.RB.ToString())
                            return true;
                    }
                }
            }
            return false;
        }    
    }
}
