using Football.Models;
using Football.Enums;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

namespace Football
{
    public class SettingsService : ISettingsService
    {
        private readonly ProjectionLimits _projectionLimits;
        private readonly BoomBustSettings _boomBust;
        private readonly WaiverWireSettings _wireSettings;
        private readonly StartOrSitSettings _startOrSitSettings;
        private readonly IMemoryCache _cache;

        public SettingsService(IOptionsMonitor<ProjectionLimits> projectionLimits, IOptionsMonitor<BoomBustSettings> boomBust, IOptionsMonitor<WaiverWireSettings> wireSettings,
             IMemoryCache cache, IOptionsMonitor<StartOrSitSettings> startOrSitSettings)
        {
            _projectionLimits = projectionLimits.CurrentValue;
            _boomBust = boomBust.CurrentValue;
            _cache = cache;
            _wireSettings = wireSettings.CurrentValue;
            _startOrSitSettings = startOrSitSettings.CurrentValue;
        }

        public int GetProjectionsCount(Position position) => position switch
        {

            Position.QB => _projectionLimits.QBProjections,
            Position.RB => _projectionLimits.RBProjections,
            Position.WR => _projectionLimits.WRProjections,
            Position.TE => _projectionLimits.TEProjections,
            Position.DST => _projectionLimits.DSTProjections,
            _ => 0
        };

        public double GetBoomSetting(Position position) => position switch
        {
            Position.QB => _boomBust.QBBoom,
            Position.RB => _boomBust.RBBoom,
            Position.WR => _boomBust.WRBoom,
            Position.TE => _boomBust.TEBoom,
            _ => 0
        };

        public double GetBustSetting(Position position) => position switch
        {
            Position.QB => _boomBust.QBBust,
            Position.RB => _boomBust.RBBust,
            Position.WR => _boomBust.WRBust,
            Position.TE => _boomBust.TEBust,
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

        public double GetValueFromModel<T>(T model, Model value)
        {
            var prop = typeof(T).GetProperties().First(p => p.ToString()!.Contains(value.ToString()));
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
            return typeof(T).GetProperties().Where(p => 
                                               !p.ToString()!.Contains(Model.PlayerId.ToString())
                                            && !p.ToString()!.Contains(Model.Season.ToString())
                                            && !p.ToString()!.Contains(Model.Week.ToString())
                                            && !p.ToString()!.Contains(Model.TeamDrafted.ToString())
                                            && !p.ToString()!.Contains(Model.RookieSeason.ToString())
                                            && !p.ToString()!.Contains(Model.Position.ToString())
                                            ).ToList();
        }

        public bool GetFromCache<T>(Cache cache, out List<T> cachedValues)
        {
            if (_cache.TryGetValue(cache.ToString(), out cachedValues))
            {
                return cachedValues.Any();
            }
            else return false;
        }

        public bool GetFromCache<T>(Position position, Cache cache, out List<T> cachedValues)
        {
            if (_cache.TryGetValue(position.ToString() + cache.ToString(), out cachedValues))
            {
                return cachedValues.Any();
            }
            else return false;
        }
        public bool GetFromCache<T>(int id, Cache cache, out T cachedValue) => _cache.TryGetValue(id.ToString() + cache.ToString(), out cachedValue);

    }
}
