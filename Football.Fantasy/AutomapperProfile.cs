using AutoMapper;
using Football.Players.Models;
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


            CreateMap<WeeklyDataQB, WeeklyFantasy>()
                     .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.QB.ToString()))
                     .ForMember(sf => sf.Games, o => o.MapFrom(sf => 1))
                     .ForMember(sf => sf.FantasyPoints, o => o.Ignore());
            CreateMap<WeeklyDataRB, WeeklyFantasy>()
                     .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.RB.ToString()))
                     .ForMember(sf => sf.Games, o => o.MapFrom(sf => 1))
                     .ForMember(sf => sf.FantasyPoints, o => o.Ignore());
            CreateMap<WeeklyDataWR, WeeklyFantasy>()
                     .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.WR.ToString()))
                     .ForMember(sf => sf.Games, o => o.MapFrom(sf => 1))
                     .ForMember(sf => sf.FantasyPoints, o => o.Ignore());
            CreateMap<WeeklyDataTE, WeeklyFantasy>()
                     .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.TE.ToString()))
                     .ForMember(sf => sf.Games, o => o.MapFrom(sf => 1))
                     .ForMember(sf => sf.FantasyPoints, o => o.Ignore());
            CreateMap<WeeklyDataDST, WeeklyFantasy>()
                     .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.DST.ToString()))
                     .ForMember(sf => sf.Games, o => o.MapFrom(sf => 1))
                     .ForMember(sf => sf.FantasyPoints, o => o.Ignore());
            CreateMap<WeeklyDataK, WeeklyFantasy>()
                    .ForMember(sf => sf.Position, o => o.MapFrom(sf => Enums.Position.K.ToString()))
                    .ForMember(sf => sf.Games, o => o.MapFrom(sf => 1))
                    .ForMember(sf => sf.FantasyPoints, o => o.Ignore());

            CreateMap<Hour, Weather>()
                    .ForMember(w => w.GameTime, o => o.MapFrom(h => h.time))
                    .ForMember(w => w.Temperature, o => o.MapFrom(h => string.Format("{0}°F", h.temp_f)))
                    .ForMember(w => w.Condition, o => o.MapFrom(h => h.condition.text))
                    .ForMember(w => w.ConditionURL, o => o.MapFrom(h => string.Format("https:{0}", h.condition.icon)))
                    .ForMember(w => w.Wind, o => o.MapFrom(h => string.Format("{0} mph winds", h.wind_mph)))
                    .ForMember(w => w.RainChance, o => o.MapFrom(h => string.Format("{0}% chance of rain", h.chance_of_rain)))
                    .ForMember(w => w.SnowChance, o => o.MapFrom(h => string.Format("{0}% chance of snow", h.chance_of_snow)));
        }
    }
}
