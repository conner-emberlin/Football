using Football.Enums;
using Football.Models;
using System.Reflection;

namespace Football
{
    public interface ISettingsService
    {
        Task<bool> UploadSeasonTunings(Tunings tunings);
        Task<Tunings?> GetSeasonTunings(int season);
        Task<bool> UploadWeeklyTunings(WeeklyTunings tunings);
        Task<WeeklyTunings> GetWeeklyTunings(int season, int week);
        Task<bool> UploadSeasonAdjustments(SeasonAdjustments seasonAdjustments);
        Task<SeasonAdjustments> GetSeasonAdjustments(int season);
        double GetBoomSetting(Position position);
        double GetBustSetting(Position position);
        double GoodWeek(Position position);
        double GetPlayerComparison(Position position);
        int GetReplacementLevel(Position position, Tunings tunings);
        double GetAverageProjectionByPosition(Position position, Tunings tunings);
        double GetValueFromModel<T>(T type, Model model);
        List<PropertyInfo> GetPropertiesFromModel<T>(List<string>? filter = null);
        IEnumerable<string> GetPropertyNamesFromModel<T>();
        bool GetFromCache<T>(Cache cache, out List<T> cachedValues);
        bool GetFromCache<T>(Position position, Cache cache, out List<T> cachedValues);
        bool GetFromCache<T>(int id, Cache cache, out T cachedValue);
    }
}
