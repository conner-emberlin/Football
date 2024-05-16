using System.Globalization;

namespace Football.UI.Helpers
{
    public class Formatter
    {
        public string PositionColor(string position)
        {
            return position switch
            {
                "QB" => "#ff2a6d",
                "RB" => "#00ceb8",
                "WR" => "#58a7ff",
                "TE" => "#ffae58",
                "DST" => "#7988a1",
                "K" => "#BD66FF",
                _ => "#000000"
            };
        }
        public int Rank<T>(List<T> list, T item) => list.IndexOf(item) + 1;

        public string ConvertDate(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return dt.ToString("MM/dd");
        }
    }
}
