using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;

namespace Football.Players.Services
{
    public class DistanceService(IPlayersService playersService, ITeamsService teamsService, IOptionsMonitor<GeoDistance> geo, IOptionsMonitor<Season> season) : IDistanceService
    {
        private readonly GeoDistance _geo = geo.CurrentValue;
        private readonly Season _season = season.CurrentValue;

        public async Task<double> GetTravelDistance(int playerId)
        {
            var currentWeek = await playersService.GetCurrentWeek(_season.CurrentSeason);
            var scheduleDetails = await playersService.GetScheduleDetails(_season.CurrentSeason, currentWeek);
            var playerTeam = await teamsService.GetPlayerTeam(_season.CurrentSeason, playerId);
            if (playerTeam != null)
            {
                var teamId = await teamsService.GetTeamId(playerTeam.Team);
                var scheduleDetail = scheduleDetails.FirstOrDefault(s => s.HomeTeamId == teamId || s.AwayTeamId == teamId);
                if (scheduleDetail == null)
                    return 0;
                else if (scheduleDetail.HomeTeamId == teamId)
                    return 0;
                else
                    return GetDistance(await playersService.GetTeamLocation(teamId), await playersService.GetTeamLocation(scheduleDetail.HomeTeamId));
            }
            else return 0;
        }
        private double GetDistance(TeamLocation location1, TeamLocation location2)
        {
            location1 = FormatCoordinates(location1);
            location2 = FormatCoordinates(location2);

            var latDiff = Radians(location1.Latitude - location2.Latitude);
            var longDiff = Radians(location1.Longitude - location2.Longitude);

            var a = Math.Pow(Math.Sin(latDiff / 2), 2)
                  + Math.Cos(Radians(location1.Latitude)) 
                  * Math.Cos(Radians(location2.Latitude))
                  * Math.Pow(Math.Sin(longDiff / 2), 2);

            return _geo.RadiusMiles * 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
        }

        private TeamLocation FormatCoordinates(TeamLocation location)
        {
            location.Longitude *= -Math.Pow(10, _geo.LongLatDec);
            location.Latitude *= Math.Pow(10, _geo.LongLatDec);
            return location;
        }
        private static double Radians(double val) => (Math.PI / 180) * val;
    }
}
