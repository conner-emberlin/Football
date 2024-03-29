using AutoMapper;
using Football.Data.Models;

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
        }


    }
}