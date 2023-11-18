using Football.Models;
using Football.Enums;
using System.Reflection;

namespace Football
{
    public interface ISettingsService
    {
        public int GetProjectionsCount(Position position);
        public double GetBoomSetting(Position position);
        public double GetBustSetting(Position position);
        public double GoodWeek(Position position);
        public double GetPlayerComparison(Position position);
        public double GetValueFromModel<T>(T type, Model model);
        public List<PropertyInfo> GetPropertiesFromModel<T>();
        public bool GetFromCache<T>(Cache cache, out List<T> cachedValues);
        public bool GetFromCache<T>(Position position, Cache cache, out List<T> cachedValues);
        public bool GetFromCache<T>(int id, Cache cache, out T cachedValue);
    }
}
