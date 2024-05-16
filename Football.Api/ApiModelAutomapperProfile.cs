using AutoMapper;
using Football.Api.Models;
using Football.Fantasy.Analysis.Models;
using Football.Players.Models;
using Football.Projections.Models;
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
                    .ReverseMap();
            CreateMap<SeasonProjection, SeasonProjectionModel>(MemberList.Destination)
                .ForMember(spm => spm.CanDelete, o => o.Ignore())
                .ForMember(spm => spm.Team, o => o.Ignore());
            CreateMap<SeasonFlex, SeasonProjectionModel>(MemberList.Destination)
                .ForMember(spm => spm.CanDelete, o => o.Ignore())
                .ForMember(spm => spm.Team, o => o.Ignore());
            CreateMap<WeekProjection, WeekProjectionModel>(MemberList.Destination)
                .ForMember(spm => spm.CanDelete, o => o.Ignore())
                .ForMember(spm => spm.Team, o => o.Ignore())
                .ForMember(spm => spm.Opponent, o => o.Ignore());
            CreateMap<MatchupRanking, MatchupRankingModel>(MemberList.Destination)
                .ForMember(mr => mr.TeamDescription, o => o.Ignore());
        }

    }
}
