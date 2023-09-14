using Football.Models;

namespace Football
{
    public interface ISettingsService
    {
        public Season GetSeason();
        public ReplacementLevels GetReplacementLevels();
        public FantasyScoring GetFantasyScoring();
        public ProjectionLimits GetProjectionLimits();
        public Starters GetStarters();
        public Tunings GetTunings();
        public WeeklyTunings GetWeeklyTunings();
    }
}
