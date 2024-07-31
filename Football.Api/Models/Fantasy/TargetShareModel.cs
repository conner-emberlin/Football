using Football.Players.Models;

namespace Football.Api.Models.Fantasy
{
    public class TargetShareModel
    {
        public int TeamId { get; set; }
        public string TeamDescription { get; set; } = "";
        public double RBTargetShare { get; set; }
        public double RBCompShare { get; set; }
        public double WRTargetShare { get; set; }
        public double WRCompShare { get; set; }
        public double TETargetShare { get; set; }
        public double TECompShare { get; set; }
    }
}
