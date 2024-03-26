using AutoMapper;
using Football.Api.Models;
using Football.Players.Models;
namespace Football.Api
{
    public class ApiModelAutomapperProfile : Profile
    {
        public ApiModelAutomapperProfile()
        {
            CreateMap<RookiePlayerModel, Player>();
            CreateMap<RookiePlayerModel, Rookie>();
        }
    }
}
