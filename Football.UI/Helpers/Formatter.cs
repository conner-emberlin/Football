using Microsoft.AspNetCore.Components;
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
        public string StartFormat(bool start) => start ? "Start" : "Sit";
        public string GetStartColor(bool start) => start ? "color:green" : "color:red";
        public string FormatMatchupRank(int rank)
        {
            return 1 <= rank && rank < 10 ? "color:red"
                  : 10 <= rank && rank < 20 ? "color:orange"
                  : "color:green";
        }

        public string AdpDiffColor(double diff)
        {
            return diff < 0 ? "color:red" : "color:green";
        }

        public string GetDiffColor(double diff)
        {
            diff *= -1;
            return diff < 3 ? "color:green"
                  : 3 <= diff && diff < 10 ? "color:orange"
                  : "color:red";
        }

        public List<string> Positions(bool includeFlex = false, bool includeQB = true, bool includeDST = true, bool includeK = true, bool includeRookies = false)
        {
            var list = new List<string> { "Quarterback", "Runningback", "Wide Receiver", "Tight End", "DST", "Kicker" };

            if (includeFlex) list.Add("Flex");
            if (includeRookies) list.Add("Rookies");
            if (!includeQB) list.Remove("Quarterback");
            if (!includeDST) list.Remove("DST");
            if (!includeK) list.Remove("Kicker");
            return list;
        }

        public string SetPosition(ChangeEventArgs? args)
        {
            if (args == null) return "QB";
            if (args.Value == null) return "QB";

            else return args.Value.ToString() switch
            {
                "Quarterback" => "QB",
                "Runningback" => "RB",
                "Wide Receiver" => "WR",
                "Tight End" => "TE",
                "DST" => "DST",
                "Kicker" => "K",
                "Flex" => "FLEX",
                _ => "QB"
            };
        }

        public string SetPosition(string position)
        {
            return position.ToLower() switch
            {
                "quarterback" => "QB",
                "runningback" => "RB",
                "wide receiver" => "WR",
                "tight end" => "TE",
                "dst" => "DST",
                "kicker" => "K",
                "flex" => "FLEX",
                _ => "QB"
            };
        }
    }
}
