using Football.Models;
using Microsoft.Extensions.Options;

namespace Football
{
    public class SettingsService : ISettingsService
    {
        private readonly Season _season;
        private readonly ReplacementLevels _replacementLevels;
        private readonly FantasyScoring _fantasyScoring;
        private readonly ProjectionLimits _projectionLimits;
        private readonly Starters _starters;
        private readonly Tunings _tunings;
        private readonly WeeklyTunings _weeklyTunings;

        public SettingsService(IOptionsMonitor<Season> season, IOptionsMonitor<ReplacementLevels> replacementLevels, IOptionsMonitor<FantasyScoring> fantasyScoring,
                    IOptionsMonitor<ProjectionLimits> projectionLimits, IOptionsMonitor<Starters> starters, IOptionsMonitor<Tunings> tunings, IOptionsMonitor<WeeklyTunings> weeklyTunings)
        {
            _season = season.CurrentValue;
            _replacementLevels = replacementLevels.CurrentValue;
            _fantasyScoring = fantasyScoring.CurrentValue;
            _projectionLimits = projectionLimits.CurrentValue;
            _starters = starters.CurrentValue;
            _tunings = tunings.CurrentValue;
            _weeklyTunings = weeklyTunings.CurrentValue;
        }

        public Season GetSeason() => _season;
        public ReplacementLevels GetReplacementLevels() => _replacementLevels;
        public FantasyScoring GetFantasyScoring() => _fantasyScoring;
        public ProjectionLimits GetProjectionLimits() => _projectionLimits;
        public Starters GetStarters() => _starters;
        public Tunings GetTunings() => _tunings;
        public WeeklyTunings GetWeeklyTunings() => _weeklyTunings;

    }
}
