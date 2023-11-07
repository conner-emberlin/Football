using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;

namespace Football.Players.Services
{
    public class DistanceService : IDistanceService
    {
        private readonly IPlayersService _playersService;
        private readonly GeoDistance _geo;
        private readonly Season _season;

        public DistanceService(IPlayersService playersService, IOptionsMonitor<GeoDistance> geo, IOptionsMonitor<Season> season)
        {
            _playersService = playersService;
            _geo = geo.CurrentValue;
            _season = season.CurrentValue;
        }
        public async Task<double> GetTravelDistance(int playerId)
        {
            var currentWeek = await _playersService.GetCurrentWeek(_season.CurrentSeason);
            var scheduleDetails = await _playersService.GetScheduleDetails(_season.CurrentSeason, currentWeek);
            var playerTeam = await _playersService.GetPlayerTeam(_season.CurrentSeason, playerId);
            if (playerTeam is not null)
            {
                var teamId = await _playersService.GetTeamId(playerTeam.Team);
                var scheduleDetail = scheduleDetails.FirstOrDefault(s => s.HomeTeamId == teamId || s.AwayTeamId == teamId);
                if (scheduleDetail is null)
                {
                    return 0;
                }
                else if (scheduleDetail.HomeTeamId == teamId)
                {
                    return 0;
                }
                else
                {
                    var location1 = await _playersService.GetTeamLocation(teamId);
                    var location2 = await _playersService.GetTeamLocation(scheduleDetail.HomeTeamId);
                    return GetDistance(location1, location2);
                }
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
            var c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

            return _geo.RadiusMiles * c;
        }

        private static TeamLocation FormatCoordinates(TeamLocation location)
        {
            location.Longitude *= -Math.Pow(10, -6);
            location.Latitude *= Math.Pow(10, -6);
            return location;
        }
        private static double Radians(double val) => (Math.PI / 180) * val;
    }
}
