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
        private readonly IMemoryCache _cache;

        public SettingsService(IOptionsMonitor<ProjectionLimits> projectionLimits, IOptionsMonitor<BoomBustSettings> boomBust, IOptionsMonitor<WaiverWireSettings> wireSettings,
             IMemoryCache cache)
        {
            _projectionLimits = projectionLimits.CurrentValue;
            _boomBust = boomBust.CurrentValue;
            _cache = cache;
            _wireSettings = wireSettings.CurrentValue;
        }

        public int GetProjectionsCount(PositionEnum position) => position switch
        {

            PositionEnum.QB => _projectionLimits.QBProjections,
            PositionEnum.RB => _projectionLimits.RBProjections,
            PositionEnum.WR => _projectionLimits.WRProjections,
            PositionEnum.TE => _projectionLimits.TEProjections,
            _ => 0
        };

        public double GetBoomSetting(PositionEnum position) => position switch
        {
            PositionEnum.QB => _boomBust.QBBoom,
            PositionEnum.RB => _boomBust.RBBoom,
            PositionEnum.WR => _boomBust.WRBoom,
            PositionEnum.TE => _boomBust.TEBoom,
            _ => 0
        };

        public double GetBustSetting(PositionEnum position) => position switch
        {
            PositionEnum.QB => _boomBust.QBBust,
            PositionEnum.RB => _boomBust.RBBust,
            PositionEnum.WR => _boomBust.WRBust,
            PositionEnum.TE => _boomBust.TEBust,
            _ => 0
        };

        public double GoodWeek(PositionEnum position)
        {
            return position switch
            {
                PositionEnum.QB => _wireSettings.GoodWeekFantasyPointsQB,
                PositionEnum.RB => _wireSettings.GoodWeekFantasyPointsRB,
                PositionEnum.WR => _wireSettings.GoodWeekFantasyPointsWR,
                PositionEnum.TE => _wireSettings.GoodWeekFantasyPointsTE,
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

        public bool GetFromCache<T>(PositionEnum position, Cache cache, out List<T> cachedValues)
        {
            if (_cache.TryGetValue(position.ToString() + cache.ToString(), out cachedValues))
            {
                return cachedValues.Any();
            }
            else return false;
        }
    }
}
