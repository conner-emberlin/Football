using Football.Models;
using Football.Enums;
using System.Reflection;
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

        public int GetProjectionsCount(PositionEnum position) => position switch
        {

            PositionEnum.QB => _projectionLimits.QBProjections,
            PositionEnum.RB => _projectionLimits.RBProjections,
            PositionEnum.WR => _projectionLimits.WRProjections,
            PositionEnum.TE => _projectionLimits.TEProjections,
            _ => 0
        };

        public double GetValueFromModel<T>(T model, Model value)
        {
            var prop = typeof(T).GetProperties().Where(p => !string.IsNullOrWhiteSpace(p.ToString()) && p.ToString().Contains(value.ToString())).First();
            if (prop != null)
            {
                return Convert.ToDouble(prop.GetValue(model));
            }
            else
            {
                return 0;
            }
        }

        public List<PropertyInfo> GetPropertiesFromModel<T>()
        {
            return typeof(T).GetProperties().Where(p => !string.IsNullOrWhiteSpace(p.ToString())
                                            && !p.ToString().Contains(Model.PlayerId.ToString())
                                            && !p.ToString().Contains(Model.Season.ToString())
                                            && !p.ToString().Contains(Model.Week.ToString())
                                            && !p.ToString().Contains(Model.TeamDrafted.ToString())
                                            && !p.ToString().Contains(Model.RookieSeason.ToString())
                                            && !p.ToString().Contains(Model.Position.ToString())
                                            ).ToList();
        }



    }
}
