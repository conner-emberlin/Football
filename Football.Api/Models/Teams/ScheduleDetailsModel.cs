namespace Football.Api.Models.Teams
{
    public class ScheduleDetailsModel
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public string Day { get; set; } = "";
        public string Date { get; set; } = "";
        public string Time { get; set; } = "";
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
    }
}
