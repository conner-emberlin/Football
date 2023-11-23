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
                _ => "#000000"
            };
        }
        public int Rank<T>(List<T> list, T item) => list.IndexOf(item) + 1;
    }
}
