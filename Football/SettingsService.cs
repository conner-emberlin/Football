﻿using Football.Models;
using Football.Enums;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Football
{
    public class SettingsService(ISettingsRepository settingsRepository, IOptionsMonitor<ProjectionLimits> projectionLimits, IOptionsMonitor<BoomBustSettings> boomBust, IOptionsMonitor<WaiverWireSettings> wireSettings,
         IMemoryCache cache, IOptionsMonitor<StartOrSitSettings> startOrSitSettings, IOptionsMonitor<ReplacementLevels> replacementLevels, IOptionsMonitor<Season> season, IOptionsMonitor<Tunings> tunings) : ISettingsService
    {
        private readonly ProjectionLimits _projectionLimits = projectionLimits.CurrentValue;
        private readonly BoomBustSettings _boomBust = boomBust.CurrentValue;
        private readonly WaiverWireSettings _wireSettings = wireSettings.CurrentValue;
        private readonly StartOrSitSettings _startOrSitSettings = startOrSitSettings.CurrentValue;
        private readonly ReplacementLevels _replacementLevels = replacementLevels.CurrentValue;
        private readonly Tunings _tunings = tunings.CurrentValue;
        private readonly int _currentSeason = season.CurrentValue.CurrentSeason;
        private readonly IMemoryCache _cache = cache;

        public async Task<bool> UploadSeasonTunings(Tunings tunings) => await settingsRepository.UploadSeasonTunings(tunings);
        public async Task<Tunings> GetSeasonTunings(int season) => await settingsRepository.GetSeasonTunings(season);
        public async Task<bool> UploadWeeklyTunings(WeeklyTunings tunings) => await settingsRepository.UploadWeeklyTunings(tunings);
        public async Task<WeeklyTunings> GetWeeklyTunings(int season, int week) => await settingsRepository.GetWeeklyTunings(season, week);
        public int GetProjectionsCount(Position position) => position switch
        {

            Position.QB => _projectionLimits.QBProjections,
            Position.RB => _projectionLimits.RBProjections,
            Position.WR => _projectionLimits.WRProjections,
            Position.TE => _projectionLimits.TEProjections,
            Position.DST => _projectionLimits.DSTProjections,
            Position.K => _projectionLimits.KProjections,
            _ => 0
        };

        public double GetBoomSetting(Position position) => position switch
        {
            Position.QB => _boomBust.QBBoom,
            Position.RB => _boomBust.RBBoom,
            Position.WR => _boomBust.WRBoom,
            Position.TE => _boomBust.TEBoom,
            Position.DST => _boomBust.DSTBoom,
            Position.K => _boomBust.KBoom,
            _ => 0
        };

        public double GetBustSetting(Position position) => position switch
        {
            Position.QB => _boomBust.QBBust,
            Position.RB => _boomBust.RBBust,
            Position.WR => _boomBust.WRBust,
            Position.TE => _boomBust.TEBust,
            Position.DST => _boomBust.DSTBust,
            Position.K => _boomBust.KBust,
            _ => 0

        };

        public double GoodWeek(Position position)
        {
            return position switch
            {
                Position.QB => _wireSettings.GoodWeekFantasyPointsQB,
                Position.RB => _wireSettings.GoodWeekFantasyPointsRB,
                Position.WR => _wireSettings.GoodWeekFantasyPointsWR,
                Position.TE => _wireSettings.GoodWeekFantasyPointsTE,
                _ => 0
            };
        }

        public double GetPlayerComparison(Position position)
        {
            return position switch
            { 
                Position.QB => _startOrSitSettings.QBCompareRange,
                Position.RB => _startOrSitSettings.RBCompareRange,
                Position.WR => _startOrSitSettings.WRCompareRange,
                Position.TE => _startOrSitSettings.TECompareRange,
                _ => 0
            };
        }

        public int GetReplacementLevel(Position position)
        {
            return position switch
            {
                Position.QB => _replacementLevels.ReplacementLevelQB,
                Position.RB => _replacementLevels.ReplacementLevelRB,
                Position.WR => _replacementLevels.ReplacementLevelWR,
                Position.TE => _replacementLevels.ReplacementLevelTE,
                _ => 0
            };
        }

        public double GetValueFromModel<T>(T model, Model value)
        {
            var prop = typeof(T).GetProperties().First(p => p.ToString()!.Contains(value.ToString()));
            return prop != null ? Convert.ToDouble(prop.GetValue(model)) : 0;
        }

        public List<PropertyInfo> GetPropertiesFromModel<T>(List<string>? filter = null)
        {
            var props = typeof(T).GetProperties().Where(p => 
                                               !p.ToString()!.Contains(Model.PlayerId.ToString())
                                            && !p.ToString()!.Contains(Model.Season.ToString())
                                            && !p.ToString()!.Contains(Model.Week.ToString())
                                            && !p.ToString()!.Contains(Model.TeamDrafted.ToString())
                                            && !p.ToString()!.Contains(Model.RookieSeason.ToString())
                                            && !p.ToString()!.Contains(Model.Position.ToString())
                                            && !p.ToString()!.Contains(Model.TeamId.ToString())
                                            ).ToList();
            return filter != null ? props.Where(p => filter.Select(f => PascalCase(f)).Contains(p.Name)).ToList() : props;
        }

        public IEnumerable<string> GetPropertyNamesFromModel<T>()
        {
            var props = GetPropertiesFromModel<T>();
            return props.Select(p => ConvertToWords(p.Name));
        }
        public bool GetFromCache<T>(Cache cache, out List<T> cachedValues) => _cache.TryGetValue(cache.ToString(), out cachedValues!) && cachedValues.Count > 0;
        public bool GetFromCache<T>(Position position, Cache cache, out List<T> cachedValues) => _cache.TryGetValue(position.ToString() + cache.ToString(), out cachedValues!) && cachedValues.Count > 0;
        public bool GetFromCache<T>(int id, Cache cache, out T cachedValue) => _cache.TryGetValue(id.ToString() + cache.ToString(), out cachedValue!);

        private string ConvertToWords(string str) => Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
        private string PascalCase(string str)
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(str).Replace(" ", "");
        }

    }
}
