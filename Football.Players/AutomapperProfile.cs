using AutoMapper;
using Football.Players.Models;

namespace Football.Players
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile() 
        {
            CreateMap<InSeasonInjury, PlayerInjury>();
            CreateMap<TeamMap, PlayerTeam>()
                .ForMember(pt => pt.Name, o => o.MapFrom(t => t.TeamDescription));
        }
    }
}
