using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Players.Interfaces;
using Football.Enums;
using Microsoft.Extensions.Options;

namespace Football.Fantasy.Services
{
    public class WaiverWireService(IStatisticsService statisticsService, IFantasyDataService fantasyService,
        ISettingsService settingsService, IPlayersService playersService, ITeamsService teamsService, IOptionsMonitor<Season> season, IOptionsMonitor<WaiverWireSettings> settings) : IWaiverWireService
    {
        private readonly WaiverWireSettings _settings = settings.CurrentValue;
        private readonly Season _season = season.CurrentValue;

        public async Task<List<WaiverWireCandidate>> GetWaiverWireCandidates(int season, int week)
        {
            List<WaiverWireCandidate> candidates = [];
            if (week > await playersService.GetCurrentSeasonWeeks()) return candidates;

            var rosterPercentages = (await statisticsService.GetWeeklyRosterPercentages(season, week - 1)).Where(r => r.RosterPercent < _settings.RostershipMax).ToList();
            foreach (var rp in rosterPercentages)
            {
                var player = await playersService.GetPlayer(rp.PlayerId);
                if (Enum.TryParse(player.Position, out Position pos) && player.Active == 1)
                {
                    var weeklyFantasy = await fantasyService.GetWeeklyFantasy(rp.PlayerId);
                    var goodWeekCount = weeklyFantasy.Count(w => w.FantasyPoints > settingsService.GoodWeek(pos));
                    var recentWeekFantasy = weeklyFantasy.FirstOrDefault(wf => wf.Week == week - 1);            
                    if((week > 2 && goodWeekCount > _settings.GoodWeekMinimum && recentWeekFantasy != null) 
                            || (week == 2 & goodWeekCount == _settings.GoodWeekMinimum && recentWeekFantasy != null))
                    {
                        if (recentWeekFantasy.FantasyPoints > settingsService.GoodWeek(pos))
                        {
                            candidates.Add(new WaiverWireCandidate
                            {
                                Player = player,
                                PlayerTeam = await teamsService.GetPlayerTeam(_season.CurrentSeason, player.PlayerId),
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
                                PlayerTeam = await teamsService.GetPlayerTeam(_season.CurrentSeason, player.PlayerId),
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
            var playerTeam = await teamsService.GetPlayerTeam(_season.CurrentSeason, playerId);            
            if (playerTeam != null)
            {
                var activeInjuries = await playersService.GetActiveInSeasonInjuries(_season.CurrentSeason);
                var otherPlayersOnTeam = (await teamsService.GetPlayersByTeam(playerTeam.Team)).Where(p => p.PlayerId != playerId);
                var injuredPlayersOnTeam = activeInjuries.Join(otherPlayersOnTeam, ai => ai.PlayerId, op => op.PlayerId, (ai, op) => new { InSeasonInjury = ai, PlayerTeam = op });                                                        
                if (injuredPlayersOnTeam.Any())
                {
                    foreach (var injuredPlayer in injuredPlayersOnTeam)
                    {
                        var position = (await playersService.GetPlayer(injuredPlayer.PlayerTeam.PlayerId)).Position;
                        if (position == Position.RB.ToString())
                            return true;
                    }
                }
            }
            return false;
        }    
    }
}
