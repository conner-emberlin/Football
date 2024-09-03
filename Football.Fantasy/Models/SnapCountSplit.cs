
namespace Football.Fantasy.Models
{
    public class SnapCountSplit
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double FirstHalfSnapsPG { get; set; }
        public double SecondHalfSnapsPG { get; set; }
    }
}
