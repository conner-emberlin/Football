using AutoMapper;
using Football.Data.Models;
using Football.Fantasy.Models;
namespace Football.Fantasy
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile() 
        {
            CreateMap<SeasonDataQB, SeasonFantasy>()
                     .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.QB.ToString()))
                     .ForMember(sf => sf.FantasyPoints, o => o.Ignore());
            CreateMap<SeasonDataRB, SeasonFantasy>()
                     .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.RB.ToString()))
                     .ForMember(sf => sf.FantasyPoints, o => o.Ignore());
            CreateMap<SeasonDataWR, SeasonFantasy>()
                     .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.WR.ToString()))
                     .ForMember(sf => sf.FantasyPoints, o => o.Ignore());
            CreateMap<SeasonDataTE, SeasonFantasy>()
                     .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.TE.ToString()))
                     .ForMember(sf => sf.FantasyPoints, o => o.Ignore());
            CreateMap<SeasonDataQB, SeasonFantasy>()
                     .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.DST.ToString()))
                     .ForMember(sf => sf.FantasyPoints, o => o.Ignore());
            CreateMap<SeasonDataQB, SeasonFantasy>()
                    .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.K.ToString()))
                    .ForMember(sf => sf.FantasyPoints, o => o.Ignore());


        }
    }
}
