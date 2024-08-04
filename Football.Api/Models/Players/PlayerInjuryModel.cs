namespace Football.Api.Models.Players
{
    public class PlayerInjuryModel
    {
        public SimplePlayerModel Player { get; set; } = new();
        public InSeasonInjuryModel InjuryModel { get; set; } = new();  
    }
}
