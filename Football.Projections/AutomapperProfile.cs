using AutoMapper;
using Football.Players.Models;
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
                .ForMember(q => q.SacksPerGame, o => o.MapFrom(q => q.Sacks / q.Games))
                .ForMember(q => q.InterceptionsPerGame, o => o.MapFrom(q => q.Int / q.Games))
                .ForMember(q => q.FumblesPerGame, o => o.MapFrom(q => q.Fumbles / q.Games));

            CreateMap<AllSeasonDataQB, QBModelSeason>()
                .ForMember(q => q.PassingAttemptsPerGame, o => o.MapFrom(q => q.Attempts / q.Games))
                .ForMember(q => q.PassingYardsPerGame, o => o.MapFrom(q => q.Yards / q.Games))
                .ForMember(q => q.PassingTouchdownsPerGame, o => o.MapFrom(q => q.TD / q.Games))
                .ForMember(q => q.RushingAttemptsPerGame, o => o.MapFrom(q => q.RushingAttempts / q.Games))
                .ForMember(q => q.RushingYardsPerGame, o => o.MapFrom(q => q.RushingYards / q.Games))
                .ForMember(q => q.RushingTouchdownsPerGame, o => o.MapFrom(q => q.RushingTD / q.Games))
                .ForMember(q => q.SacksPerGame, o => o.MapFrom(q => q.Sacks / q.Games))
                .ForMember(q => q.InterceptionsPerGame, o => o.MapFrom(q => q.Int / q.Games))
                .ForMember(q => q.FumblesPerGame, o => o.MapFrom(q => q.Fumbles / q.Games));

            CreateMap<WeeklyDataQB, QBModelWeek>()
                .ForMember(q => q.PassingAttemptsPerGame, o => o.MapFrom(q => q.Attempts))
                .ForMember(q => q.PassingCompletionsPerGame, o => o.MapFrom(q => q.Completions))
                .ForMember(q => q.PassingYardsPerAttempt, o => o.MapFrom(q => q.Attempts > 0 ? q.Yards / q.Attempts : 0))
                .ForMember(q => q.RushingAttemptsPerGame, o => o.MapFrom(q => q.RushingAttempts))
                .ForMember(q => q.RushingYardsPerAttempt, o => o.MapFrom(q => q.RushingAttempts > 0 ? q.RushingYards / q.RushingAttempts : 0))
                .ForMember(q => q.SacksPerGame, o => o.MapFrom(q => q.Sacks));

            CreateMap<AllWeeklyDataQB, QBModelWeek>()
                .ForMember(q => q.PassingAttemptsPerGame, o => o.MapFrom(q => q.Attempts))
                .ForMember(q => q.PassingCompletionsPerGame, o => o.MapFrom(q => q.Completions))
                .ForMember(q => q.PassingYardsPerAttempt, o => o.MapFrom(q => q.Attempts > 0 ? q.Yards/q.Attempts : 0))
                .ForMember(q => q.RushingAttemptsPerGame, o => o.MapFrom(q => q.RushingAttempts))
                .ForMember(q => q.RushingYardsPerAttempt, o => o.MapFrom(q => q.RushingAttempts > 0 ? q.RushingYards/q.RushingAttempts : 0))
                .ForMember(q => q.SacksPerGame, o => o.MapFrom(q => q.Sacks))
                .ForMember(q => q.SnapsPerGame, o => o.MapFrom(q => q.Snaps));

            CreateMap<SeasonDataRB, RBModelSeason>()
                .ForMember(q => q.RushingAttemptsPerGame, q => q.MapFrom(q => q.RushingAtt / q.Games))
                .ForMember(q => q.RushingYardsPerGame, q => q.MapFrom(q => q.RushingYds / q.Games))
                .ForMember(q => q.RushingTouchdownsPerGame, q => q.MapFrom(q => q.RushingTD / q.Games))
                .ForMember(q => q.RushingYardsPerAttempt, q => q.MapFrom(q => q.RushingAtt > 0 ? q.RushingYds / q.RushingAtt : 0))
                .ForMember(q => q.ReceivingTouchdownsPerGame, q => q.MapFrom(q => q.ReceivingTD / q.Games))
                .ForMember(q => q.ReceivingYardsPerGame, q => q.MapFrom(q => q.Yards / q.Games))
                .ForMember(q => q.ReceptionsPerGame, q => q.MapFrom(q => q.Receptions / q.Games))
                .ForMember(q => q.FumblesPerGame, o => o.MapFrom(q => q.Fumbles / q.Games));

            CreateMap<AllSeasonDataRB, RBModelSeason>()
                .ForMember(q => q.RushingAttemptsPerGame, q => q.MapFrom(q => q.RushingAtt / q.Games))
                .ForMember(q => q.RushingYardsPerGame, q => q.MapFrom(q => q.RushingYds / q.Games))
                .ForMember(q => q.RushingTouchdownsPerGame, q => q.MapFrom(q => q.RushingTD / q.Games))
                .ForMember(q => q.RushingYardsPerAttempt, q => q.MapFrom(q => q.RushingAtt > 0 ? q.RushingYds / q.RushingAtt : 0))
                .ForMember(q => q.ReceivingTouchdownsPerGame, q => q.MapFrom(q => q.ReceivingTD / q.Games))
                .ForMember(q => q.ReceivingYardsPerGame, q => q.MapFrom(q => q.Yards / q.Games))
                .ForMember(q => q.ReceptionsPerGame, q => q.MapFrom(q => q.Receptions / q.Games))
                .ForMember(q => q.FumblesPerGame, o => o.MapFrom(q => q.Fumbles / q.Games));

            CreateMap<WeeklyDataRB, RBModelWeek>()
                .ForMember(q => q.RushingAttemptsPerGame, q => q.MapFrom(q => q.RushingAtt))
                .ForMember(q => q.RushingYardsPerAttempt, q => q.MapFrom(q => q.RushingAtt > 0 ? q.RushingYds / q.RushingAtt : 0))
                .ForMember(q => q.TargetsPerGame, o => o.MapFrom(q => q.Targets))
                .ForMember(q => q.ReceivingYardsPerReception, o => o.MapFrom(q => q.Receptions > 0 ? q.Yards/q.Receptions : 0));

            CreateMap<AllWeeklyDataRB, RBModelWeek>()
                .ForMember(q => q.RushingAttemptsPerGame, q => q.MapFrom(q => q.RushingAtt))
                .ForMember(q => q.RushingYardsPerAttempt, q => q.MapFrom(q => q.RushingAtt > 0 ? q.RushingYds / q.RushingAtt : 0))
                .ForMember(q => q.TargetsPerGame, o => o.MapFrom(q => q.Targets))
                .ForMember(q => q.ReceivingYardsPerReception, o => o.MapFrom(q => q.Receptions > 0 ? q.Yards / q.Receptions : 0))
                .ForMember(q => q.SnapsPerGame, o => o.MapFrom(q => q.Snaps));

            CreateMap<SeasonDataWR, WRModelSeason>()
                .ForMember(q => q.TargetsPerGame, o => o.MapFrom(q => q.Targets / q.Games))
                .ForMember(q => q.ReceptionsPerGame, o => o.MapFrom(q => q.Receptions / q.Games))
                .ForMember(q => q.YardsPerGame, o => o.MapFrom(q => q.Yards / q.Games))
                .ForMember(q => q.YardsPerReception, o => o.MapFrom(q => q.Receptions > 0 ? q.Yards / q.Receptions : 0))
                .ForMember(q => q.TouchdownsPerGame, o => o.MapFrom(q => q.TD / q.Games))
                .ForMember(q => q.FumblesPerGame, o => o.MapFrom(q => q.Fumbles / q.Games));

            CreateMap<WeeklyDataWR, WRModelWeek>()
                .ForMember(q => q.RushingAttemptsPerGame, q => q.MapFrom(q => q.RushingAtt))
                .ForMember(q => q.RushingYardsPerAttempt, q => q.MapFrom(q => q.RushingAtt > 0 ? q.RushingYds / q.RushingAtt : 0))
                .ForMember(q => q.TargetsPerGame, o => o.MapFrom(q => q.Targets))
                .ForMember(q => q.ReceivingYardsPerReception, o => o.MapFrom(q => q.Receptions > 0 ? q.Yards / q.Receptions : 0))
                .ForMember(q => q.ReceptionRate, o => o.MapFrom(q => q.Targets > 0 ? q.Receptions/q.Targets : 0));

            CreateMap<AllWeeklyDataWR, WRModelWeek>()
                .ForMember(q => q.RushingAttemptsPerGame, q => q.MapFrom(q => q.RushingAtt))
                .ForMember(q => q.RushingYardsPerAttempt, q => q.MapFrom(q => q.RushingAtt > 0 ? q.RushingYds / q.RushingAtt : 0))
                .ForMember(q => q.TargetsPerGame, o => o.MapFrom(q => q.Targets))
                .ForMember(q => q.ReceivingYardsPerReception, o => o.MapFrom(q => q.Receptions > 0 ? q.Yards / q.Receptions : 0))
                .ForMember(q => q.SnapsPerGame, o => o.MapFrom(q => q.Snaps))
                .ForMember(q => q.ReceptionRate, o => o.MapFrom(q => q.Targets > 0 ? q.Receptions / q.Targets : 0));

            CreateMap<AllSeasonDataWR, WRModelSeason>()
                .ForMember(q => q.TargetsPerGame, o => o.MapFrom(q => q.Targets / q.Games))
                .ForMember(q => q.ReceptionsPerGame, o => o.MapFrom(q => q.Receptions / q.Games))
                .ForMember(q => q.YardsPerGame, o => o.MapFrom(q => q.Yards / q.Games))
                .ForMember(q => q.YardsPerReception, o => o.MapFrom(q => q.Receptions > 0 ? q.Yards / q.Receptions : 0))
                .ForMember(q => q.TouchdownsPerGame, o => o.MapFrom(q => q.TD / q.Games))
                .ForMember(q => q.FumblesPerGame, o => o.MapFrom(q => q.Fumbles / q.Games));

            CreateMap<SeasonDataTE, TEModelSeason>()
                .ForMember(q => q.TargetsPerGame, o => o.MapFrom(q => q.Targets / q.Games))
                .ForMember(q => q.ReceptionsPerGame, o => o.MapFrom(q => q.Receptions / q.Games))
                .ForMember(q => q.YardsPerGame, o => o.MapFrom(q => q.Yards / q.Games))
                .ForMember(q => q.YardsPerReception, o => o.MapFrom(q => q.Receptions > 0 ? q.Yards / q.Receptions : 0))
                .ForMember(q => q.TouchdownsPerGame, o => o.MapFrom(q => q.TD / q.Games))
                .ForMember(q => q.FumblesPerGame, o => o.MapFrom(q => q.Fumbles / q.Games));

            CreateMap<AllSeasonDataTE, TEModelSeason>()
                .ForMember(q => q.TargetsPerGame, o => o.MapFrom(q => q.Targets / q.Games))
                .ForMember(q => q.ReceptionsPerGame, o => o.MapFrom(q => q.Receptions / q.Games))
                .ForMember(q => q.YardsPerGame, o => o.MapFrom(q => q.Yards / q.Games))
                .ForMember(q => q.YardsPerReception, o => o.MapFrom(q => q.Receptions > 0 ? q.Yards / q.Receptions : 0))
                .ForMember(q => q.TouchdownsPerGame, o => o.MapFrom(q => q.TD / q.Games))
                .ForMember(q => q.FumblesPerGame, o => o.MapFrom(q => q.Fumbles / q.Games));

            CreateMap<WeeklyDataTE, TEModelWeek>()
                .ForMember(q => q.RushingAttemptsPerGame, q => q.MapFrom(q => q.RushingAtt))
                .ForMember(q => q.RushingYardsPerAttempt, q => q.MapFrom(q => q.RushingAtt > 0 ? q.RushingYds / q.RushingAtt : 0))
                .ForMember(q => q.TargetsPerGame, o => o.MapFrom(q => q.Targets))
                .ForMember(q => q.ReceivingYardsPerReception, o => o.MapFrom(q => q.Receptions > 0 ? q.Yards / q.Receptions : 0))
                .ForMember(q => q.ReceptionRate, o => o.MapFrom(q => q.Targets > 0 ? q.Receptions / q.Targets : 0));

            CreateMap<AllWeeklyDataTE, TEModelWeek>()
                .ForMember(q => q.RushingAttemptsPerGame, q => q.MapFrom(q => q.RushingAtt))
                .ForMember(q => q.RushingYardsPerAttempt, q => q.MapFrom(q => q.RushingAtt > 0 ? q.RushingYds / q.RushingAtt : 0))
                .ForMember(q => q.TargetsPerGame, o => o.MapFrom(q => q.Targets))
                .ForMember(q => q.ReceivingYardsPerReception, o => o.MapFrom(q => q.Receptions > 0 ? q.Yards / q.Receptions : 0))
                .ForMember(q => q.SnapsPerGame, o => o.MapFrom(q => q.Snaps))
                .ForMember(q => q.ReceptionRate, o => o.MapFrom(q => q.Targets > 0 ? q.Receptions / q.Targets : 0));


            CreateMap<WeeklyDataK, KModelWeek>()
                .ForMember(k => k.ExtraPointsPerGame, o => o.MapFrom(k => k.ExtraPoints))
                .ForMember(k => k.ExtraPointAttsPerGame, o => o.MapFrom(k => k.ExtraPointAttempts))
                .ForMember(k => k.FieldGoalsPerGame, o => o.MapFrom(k => k.FieldGoals))
                .ForMember(k => k.FieldGoalAttemptsPerGame, o => o.MapFrom(k => k.FieldGoalAttempts))
                .ForMember(k => k.FiftyPlusFieldGoalsPerGame, o => o.MapFrom(k => k.Fifty));

            CreateMap<AllWeeklyDataK, KModelWeek>()
                .ForMember(k => k.ExtraPointsPerGame, o => o.MapFrom(k => k.ExtraPoints))
                .ForMember(k => k.ExtraPointAttsPerGame, o => o.MapFrom(k => k.ExtraPointAttempts))
                .ForMember(k => k.FieldGoalsPerGame, o => o.MapFrom(k => k.FieldGoals))
                .ForMember(k => k.FieldGoalAttemptsPerGame, o => o.MapFrom(k => k.FieldGoalAttempts))
                .ForMember(k => k.FiftyPlusFieldGoalsPerGame, o => o.MapFrom(k => k.Fifty));

            CreateMap<WeeklyDataDST, DSTModelWeek>()
                .ForMember(d => d.SacksPerGame, o => o.MapFrom(d => d.Sacks))
                .ForMember(d => d.IntsPerGame, o => o.MapFrom(d => d.Ints))
                .ForMember(d => d.FumRecPerGame, o => o.MapFrom(d => d.FumblesRecovered))
                .ForMember(d => d.TotalTDPerGame, o => o.MapFrom(d => d.DefensiveTD + d.SpecialTD))
                .ForMember(d => d.SaftiesPerGame, o => o.MapFrom(d => d.Safties));

            CreateMap<AllWeeklyDataDST, DSTModelWeek>()
                .ForMember(d => d.SacksPerGame, o => o.MapFrom(d => d.Sacks))
                .ForMember(d => d.IntsPerGame, o => o.MapFrom(d => d.Ints))
                .ForMember(d => d.FumRecPerGame, o => o.MapFrom(d => d.FumblesRecovered))
                .ForMember(d => d.TotalTDPerGame, o => o.MapFrom(d => d.DefensiveTD + d.SpecialTD))
                .ForMember(d => d.SaftiesPerGame, o => o.MapFrom(d => d.Safties));

        }
    }
}
