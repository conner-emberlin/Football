using AutoMapper;
using Football.Models;
using Football.Data.Models;
using Football.Players.Models;
using Microsoft.Extensions.Options;

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
            CreateMap<FantasyProsStringParseDST, SeasonDataDST>();
            CreateMap<FantasyProsStringParseQB, WeeklyDataQB>();
            CreateMap<FantasyProsStringParseRB, WeeklyDataRB>();
            CreateMap<FantasyProsStringParseWR, WeeklyDataWR>();
            CreateMap<FantasyProsStringParseTE, WeeklyDataTE>();
            CreateMap<FantasyProsStringParseDST, WeeklyDataDST>();
            CreateMap<FantasyProsStringParseK, WeeklyDataK>();


            CreateMap<FantasyProsStringParseQB, Player>()
                .ForMember(p => p.Position, o => o.MapFrom(fp => Football.Enums.Position.QB.ToString()))
                .ForMember(p => p.Active, o => o.MapFrom(fp => 1));
            CreateMap<FantasyProsStringParseRB, Player>()
                .ForMember(p => p.Position, o => o.MapFrom(fp => Football.Enums.Position.RB.ToString()))
                .ForMember(p => p.Active, o => o.MapFrom(fp => 1));
            CreateMap<FantasyProsStringParseWR, Player>()
                .ForMember(p => p.Position, o => o.MapFrom(fp => Football.Enums.Position.WR.ToString()))
                .ForMember(p => p.Active, o => o.MapFrom(fp => 1));
            CreateMap<FantasyProsStringParseTE, Player>()
                .ForMember(p => p.Position, o => o.MapFrom(fp => Football.Enums.Position.TE.ToString()))
                .ForMember(p => p.Active, o => o.MapFrom(fp => 1)); 
            CreateMap<FantasyProsStringParseDST, Player>()
                .ForMember(p => p.Position, o => o.MapFrom(fp => Football.Enums.Position.DST.ToString()))
                .ForMember(p => p.Active, o => o.MapFrom(fp => 1));

        }


    }
}