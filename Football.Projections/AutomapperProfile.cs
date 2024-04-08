using AutoMapper;
using Football.Data.Models;
using Football.Projections.Models;

namespace Football.Projections
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile() 
        {
            CreateMap<SeasonDataQB, QBModelSeason>()
                .ForMember(q => q.PassingAttemptsPerGame, o => o.MapFrom(q => q.Attempts / q.Games))
                .ForMember(q => q.PassingYardsPerGame, o => o.MapFrom(q => q.Yards / q.Games))
                .ForMember(q => q.PassingTouchdownsPerGame, o => o.MapFrom(q => q.TD / q.Games))
                .ForMember(q => q.RushingAttemptsPerGame, o => o.MapFrom(q => q.RushingAttempts / q.Games))
                .ForMember(q => q.RushingYardsPerGame, o => o.MapFrom(q => q.RushingYards / q.Games))
                .ForMember(q => q.RushingTouchdownsPerGame, o => o.MapFrom(q => q.RushingTD / q.Games))
                .ForMember(q => q.SacksPerGame, o => o.MapFrom(q => q.Sacks / q.Games));

            CreateMap<WeeklyDataK, KModelWeek>()
                .ForMember(k => k.ExtraPointsPerGame, o => o.MapFrom(k => k.ExtraPoints))
                .ForMember(k => k.ExtraPointAttsPerGame, o => o.MapFrom(k => k.ExtraPointAttempts))
                .ForMember(k => k.FieldGoalsPerGame, o => o.MapFrom(k => k.FieldGoals))
                .ForMember(k => k.FieldGoalAttemptsPerGame, o => o.MapFrom(k => k.FieldGoalAttempts))
                .ForMember(k => k.FiftyPlusFieldGoalsPerGame, o => o.MapFrom(k => k.Fifty));
                
        }
    }
}
