using AutoMapper;
using Football.Players.Models;

namespace Football.Players
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile() 
        {
            CreateMap<InSeasonInjury, PlayerInjury>();        
        }
    }
}
