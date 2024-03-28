using AutoMapper;
using Football.Api.Models;
using Football.Players.Models;
namespace Football.Api
{
    public class ApiModelAutomapperProfile : Profile
    {
        public ApiModelAutomapperProfile()
        {
            CreateMap<RookiePlayerModel, Player>()
                    .ReverseMap()
                    .ForMember(rpm => rpm.DraftPosition, o => o.Ignore())
                    .ForMember(rpm => rpm.TeamDrafted, o => o.Ignore())
                    .ForMember(rpm => rpm.DeclareAge, o => o.Ignore())
                    .ForMember(rpm => rpm.RookieSeason, o => o.Ignore());
            CreateMap<RookiePlayerModel, Rookie>()
                    .ReverseMap()
                    .ForMember(rpm => rpm.Name, o => o.Ignore())
                    .ForMember(rpm => rpm.Active, o => o.Ignore());
        }
    }
}
