using AutoMapper;
using Football.Data.Models;
using Football.Players.Models;

namespace Football.Data
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<FantasyProsStringParseQB, SeasonDataQB>();
            CreateMap<FantasyProsStringParseRB, SeasonDataRB>();
            CreateMap<FantasyProsStringParseWR, SeasonDataWR>();
            CreateMap<FantasyProsStringParseTE, SeasonDataTE>();
            CreateMap<FantasyProsStringParseQB, Player>()
                .ForMember(p => p.Position, o => o.MapFrom(fp => Football.Enums.Position.TE.ToString()))
                .ForMember(p => p.Active, o => o.MapFrom(fp => 1));
            CreateMap<FantasyProsStringParseRB, Player>()
                .ForMember(p => p.Position, o => o.MapFrom(fp => Football.Enums.Position.TE.ToString()))
                .ForMember(p => p.Active, o => o.MapFrom(fp => 1));
            CreateMap<FantasyProsStringParseWR, Player>()
                .ForMember(p => p.Position, o => o.MapFrom(fp => Football.Enums.Position.TE.ToString()))
                .ForMember(p => p.Active, o => o.MapFrom(fp => 1));
            CreateMap<FantasyProsStringParseTE, Player>()
                .ForMember(p => p.Position, o => o.MapFrom(fp => Football.Enums.Position.TE.ToString()))
                .ForMember(p => p.Active, o => o.MapFrom(fp => 1)); 
            CreateMap<FantasyProsStringParseQB, Player>()
                .ForMember(p => p.Position, o => o.MapFrom(fp => Football.Enums.Position.TE.ToString()))
                .ForMember(p => p.Active, o => o.MapFrom(fp => 1));

        }


    }
}