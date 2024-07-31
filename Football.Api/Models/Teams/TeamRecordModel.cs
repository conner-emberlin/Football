namespace Football.Api.Models.Teams
{
    public class TeamRecordModel
    {
        public int TeamId { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
    }
}
