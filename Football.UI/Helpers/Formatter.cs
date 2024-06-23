﻿using System.Globalization;

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
        public string StartFormat(bool start) => start ? "Start" : "Sit";
        public string GetStartColor(bool start) => start ? "color:green" : "color:red";
        public string FormatMatchupRank(int rank)
        {
            return 1 <= rank && rank < 10 ? "color:red"
                  : 10 <= rank && rank < 20 ? "color:orange"
                  : "color:green";
        }

        public string GetDiffColor(double diff)
        {
            diff *= -1;
            return diff < 3 ? "color:green"
                  : 3 <= diff && diff < 10 ? "color:orange"
                  : "color:red";
        }
    }
}
