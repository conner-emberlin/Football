namespace Football.Projections.Models
{
    public class SeasonProjectionConfiguration
    {
        public int SeasonProjectionConfigurationId { get; set; }
        public int Season { get; set; }
        public string Position { get; set; } = "";
        public DateTime DateCreated { get; set; }
        public string Filter { get; set; } = "";
    }
}
