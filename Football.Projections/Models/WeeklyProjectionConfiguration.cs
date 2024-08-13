
namespace Football.Projections.Models
{
    public class WeeklyProjectionConfiguration
    {
        public int WeeklyProjectionConfigurationId { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public string Position { get; set; } = "";
        public DateTime DateCreated { get; set; }
        public string Filter { get; set; } = "";
    }
}
