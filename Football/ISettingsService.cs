using Football.Models;
using Football.Enums;
using System.Reflection;

namespace Football
{
    public interface ISettingsService
    {
        public int GetProjectionsCount(PositionEnum position);
        public double GetBoomSetting(PositionEnum position);
        public double GetBustSetting(PositionEnum position);
        public double GoodWeek(PositionEnum position);
        public double GetValueFromModel<T>(T type, Model model);
        public List<PropertyInfo> GetPropertiesFromModel<T>();
        public bool GetFromCache<T>(Cache cache, out List<T> cachedValues);
        public bool GetFromCache<T>(PositionEnum position, Cache cache, out List<T> cachedValues);
    }
}
