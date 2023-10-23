using Football.Models;
using Football.Enums;
using System.Reflection;

namespace Football
{
    public interface ISettingsService
    {
        public int GetProjectionsCount(PositionEnum position);
        public double GetValueFromModel<T>(T type, Model model);
        public List<PropertyInfo> GetPropertiesFromModel<T>();
    }
}
