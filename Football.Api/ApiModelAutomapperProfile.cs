using AutoMapper;
using Football.Enums;
using Football.Models;
using Football.Fantasy.Models;
using Football.Players.Models;
using Football.Projections.Models;
using Football.Shared.Models.Fantasy;
using Football.Shared.Models.Players;
using Football.Shared.Models.Operations;
using Football.Shared.Models.Projection;
using Football.Shared.Models.Teams;

namespace Football.Api
{
    public class ApiModelAutomapperProfile : Profile
    {
        public ApiModelAutomapperProfile()
        {
            CreateMap<RookiePlayerModel, Player>()
                    .ReverseMap()
                    .ForMember(rpm => rpm.DraftPick, o => o.Ignore())
                    .ForMember(rpm => rpm.TeamDrafted, o => o.Ignore())
                    .ForMember(rpm => rpm.DeclareAge, o => o.Ignore())
                    .ForMember(rpm => rpm.RookieSeason, o => o.Ignore());
            CreateMap<RookiePlayerModel, Rookie>()
                    .ReverseMap();
            CreateMap<SeasonProjection, SeasonProjectionModel>(MemberList.Destination)
                .ForMember(spm => spm.CanDelete, o => o.Ignore())
                .ForMember(spm => spm.Team, o => o.Ignore());
            CreateMap<SeasonFlex, SeasonProjectionModel>();
            CreateMap<SeasonFlex, SeasonFlexExportModel>();
            CreateMap<WeekProjection, WeekProjectionModel>(MemberList.Destination)
                .ForMember(spm => spm.CanDelete, o => o.Ignore())
                .ForMember(spm => spm.Team, o => o.Ignore())
                .ForMember(spm => spm.Opponent, o => o.Ignore());
            CreateMap<MatchupRanking, MatchupRankingModel>(MemberList.Destination)
                .ForMember(mr => mr.TeamDescription, o => o.Ignore());
            CreateMap<FantasyPerformance, FantasyAnalysisModel>(MemberList.Destination)
                .ForMember(f => f.BoomPercentage, o => o.Ignore())
                .ForMember(f => f.BustPercentage, o => o.Ignore());

            CreateMap<WeeklyDataQB, WeeklyDataModel>()
                .ForMember(w => w.Position, o => o.MapFrom(w => Position.QB.ToString()))
                .ForMember(w => w.Interceptions, o => o.MapFrom(w => w.Int))
                .ForMember(w => w.PassingYards, o => o.MapFrom(w => w.Yards))
                .ForMember(w => w.PassingTouchdowns, o => o.MapFrom(w => w.TD))
                .ForMember(w => w.RushingTouchdowns, o => o.MapFrom(w => w.RushingTD));
            CreateMap<WeeklyDataRB, WeeklyDataModel>()
                .ForMember(w => w.Position, o => o.MapFrom(w => Position.RB.ToString()))
                .ForMember(w => w.RushingAttempts, o => o.MapFrom(w => w.RushingAtt))
                .ForMember(w => w.RushingYards, o => o.MapFrom(w => w.RushingYds))
                .ForMember(w => w.RushingTouchdowns, o => o.MapFrom(w => w.RushingTD))
                .ForMember(w => w.ReceivingYards, o => o.MapFrom(w => w.Yards))
                .ForMember(w => w.ReceivingTouchdowns, o => o.MapFrom(w => w.ReceivingTD));
            CreateMap<WeeklyDataWR, WeeklyDataModel>()
                .ForMember(w => w.Position, o => o.MapFrom(w => Position.WR.ToString()))
                .ForMember(w => w.ReceivingYards, o => o.MapFrom(w => w.Yards))
                .ForMember(w => w.ReceivingTouchdowns, o => o.MapFrom(w => w.TD))
                .ForMember(w => w.RushingAttempts, o => o.MapFrom(w => w.RushingAtt))
                .ForMember(w => w.RushingTouchdowns, o => o.MapFrom(w => w.RushingTD))
                .ForMember(W => W.RushingYards, o => o.MapFrom(w => w.RushingYds));
            CreateMap<WeeklyDataTE, WeeklyDataModel>()
                .ForMember(w => w.Position, o => o.MapFrom(w => Position.TE.ToString()))
                .ForMember(w => w.ReceivingYards, o => o.MapFrom(w => w.Yards))
                .ForMember(w => w.ReceivingTouchdowns, o => o.MapFrom(w => w.TD))
                .ForMember(w => w.RushingAttempts, o => o.MapFrom(w => w.RushingAtt))
                .ForMember(w => w.RushingTouchdowns, o => o.MapFrom(w => w.RushingTD))
                .ForMember(W => W.RushingYards, o => o.MapFrom(w => w.RushingYds));
            CreateMap<WeeklyDataDST, WeeklyDataModel>()
                .ForMember(w => w.Position, o => o.MapFrom(w => Position.DST.ToString()))
                .ForMember(w => w.DefensiveSacks, o => o.MapFrom(w => w.Sacks))
                .ForMember(w => w.DefensiveSafties, o => o.MapFrom(w => w.Safties))
                .ForMember(w => w.DefensiveInterceptions, o => o.MapFrom(w => w.Ints))
                .ForMember(w => w.DefensiveTouchdowns, o => o.MapFrom(w => w.DefensiveTD))
                .ForMember(w => w.SpecialTeamsTouchdowns, o => o.MapFrom(w => w.SpecialTD));
            CreateMap<WeeklyDataK, WeeklyDataModel>()
                .ForMember(w => w.Position, o => o.MapFrom(w => Position.K.ToString()));

            CreateMap<SeasonDataQB, SeasonDataModel>()
                .ForMember(w => w.Position, o => o.MapFrom(w => Position.QB.ToString()))
                .ForMember(w => w.Interceptions, o => o.MapFrom(w => w.Int))
                .ForMember(w => w.PassingYards, o => o.MapFrom(w => w.Yards))
                .ForMember(w => w.PassingTouchdowns, o => o.MapFrom(w => w.TD))
                .ForMember(w => w.RushingTouchdowns, o => o.MapFrom(w => w.RushingTD));
            CreateMap<SeasonDataRB, SeasonDataModel>()
                .ForMember(w => w.Position, o => o.MapFrom(w => Position.RB.ToString()))
                .ForMember(w => w.RushingAttempts, o => o.MapFrom(w => w.RushingAtt))
                .ForMember(w => w.RushingYards, o => o.MapFrom(w => w.RushingYds))
                .ForMember(w => w.RushingTouchdowns, o => o.MapFrom(w => w.RushingTD))
                .ForMember(w => w.ReceivingYards, o => o.MapFrom(w => w.Yards))
                .ForMember(w => w.ReceivingTouchdowns, o => o.MapFrom(w => w.ReceivingTD));
            CreateMap<SeasonDataWR, SeasonDataModel>()
                .ForMember(w => w.Position, o => o.MapFrom(w => Position.WR.ToString()))
                .ForMember(w => w.ReceivingYards, o => o.MapFrom(w => w.Yards))
                .ForMember(w => w.ReceivingTouchdowns, o => o.MapFrom(w => w.TD))
                .ForMember(w => w.RushingAttempts, o => o.MapFrom(w => w.RushingAtt))
                .ForMember(w => w.RushingTouchdowns, o => o.MapFrom(w => w.RushingTD))
                .ForMember(W => W.RushingYards, o => o.MapFrom(w => w.RushingYds));
            CreateMap<SeasonDataTE, SeasonDataModel>()
                .ForMember(w => w.Position, o => o.MapFrom(w => Position.TE.ToString()))
                .ForMember(w => w.ReceivingYards, o => o.MapFrom(w => w.Yards))
                .ForMember(w => w.ReceivingTouchdowns, o => o.MapFrom(w => w.TD))
                .ForMember(w => w.RushingAttempts, o => o.MapFrom(w => w.RushingAtt))
                .ForMember(w => w.RushingTouchdowns, o => o.MapFrom(w => w.RushingTD))
                .ForMember(W => W.RushingYards, o => o.MapFrom(w => w.RushingYds));
            CreateMap<SeasonDataDST, SeasonDataModel>()
                .ForMember(w => w.Position, o => o.MapFrom(w => Position.DST.ToString()))
                .ForMember(w => w.DefensiveSacks, o => o.MapFrom(w => w.Sacks))
                .ForMember(w => w.DefensiveSafties, o => o.MapFrom(w => w.Safties))
                .ForMember(w => w.DefensiveInterceptions, o => o.MapFrom(w => w.Ints))
                .ForMember(w => w.DefensiveTouchdowns, o => o.MapFrom(w => w.DefensiveTD))
                .ForMember(w => w.SpecialTeamsTouchdowns, o => o.MapFrom(w => w.SpecialTD));
            CreateMap<SeasonFantasy, SeasonFantasyModel>()
                .ForMember(s => s.FantasyPointsPerGame, o => o.MapFrom(s => s.Games > 0 ? s.FantasyPoints / s.Games : 0));
            CreateMap<StartOrSit, StartOrSitModel>()
                .ForMember(s => s.Name, o => o.MapFrom(s => s.Player.Name))
                .ForMember(s => s.Position, o => o.MapFrom(s => s.Player.Position))
                .ForMember(s => s.Team, o => o.MapFrom(s => s.TeamMap != null ? s.TeamMap.Team : string.Empty))
                .ForMember(s => s.TeamId, o => o.MapFrom(s => s.TeamMap != null ? s.TeamMap.TeamId : 0))
                .ForMember(s => s.OverUnder, o => o.MapFrom(s => s.MatchLines != null ? s.MatchLines.OverUnder : 0))
                .ForMember(s => s.Line, o => o.MapFrom(s => s.MatchLines != null ? s.MatchLines.Line : 0))
                .ForMember(s => s.ImpliedTeamTotal, o => o.MapFrom(s => s.MatchLines != null ? s.MatchLines.ImpliedTeamTotal : 0))
                .ForMember(s => s.GameTime, o => o.MapFrom(s => s.Weather != null ? s.Weather.GameTime : string.Empty))
                .ForMember(s => s.Temperature, o => o.MapFrom(s => s.Weather != null ? s.Weather.Temperature : string.Empty))
                .ForMember(s => s.Wind, o => o.MapFrom(s => s.Weather != null ? s.Weather.Wind : string.Empty))
                .ForMember(s => s.Condition, o => o.MapFrom(s => s.Weather != null ? s.Weather.Condition : string.Empty))
                .ForMember(s => s.ConditionURL, o => o.MapFrom(s => s.Weather != null ? s.Weather.ConditionURL : string.Empty));
            CreateMap<PlayerComparison, PlayerComparisonModel>()
                .ForMember(p => p.Name, o => o.MapFrom(p => p.Player.Name))
                .ForMember(p => p.FantasyPoints, o => o.MapFrom(p => p.WeeklyFantasy != null ? p.WeeklyFantasy.FantasyPoints : 0))
                .ForMember(p => p.ProjectedPoints, o => o.MapFrom(p => p.ProjectedPoints));
            CreateMap<SnapCountAnalysis, SnapCountAnalysisModel>()
                .ForMember(s => s.Name, o => o.MapFrom(s => s.Player.Name))
                .ForMember(s => s.Position, o => o.MapFrom(s => s.Player.Position))
                .ForMember(s => s.PlayerId, o => o.MapFrom(s => s.Player.PlayerId));
            CreateMap<TargetShare, TargetShareModel>()
                .ForMember(t => t.TeamDescription, o => o.MapFrom(t => t.Team.TeamDescription))
                .ForMember(t => t.TeamId, o => o.MapFrom(t => t.Team.TeamId));
            CreateMap<WeeklyFantasy, WeeklyFantasyModel>();
            CreateMap<SeasonProjectionError, SeasonProjectionErrorModel>()
                .ForMember(s => s.PlayerId, o => o.MapFrom(s => s.Player.PlayerId));
            CreateMap<WeeklyProjectionAnalysis, WeeklyProjectionAnalysisModel>();
            CreateMap<SeasonProjectionAnalysis, SeasonProjectionAnalysisModel>();
            CreateMap<FantasyPercentage, FantasyPercentageModel>();
            CreateMap<MarketShare, MarketShareModel>()
                .ForMember(m => m.PlayerId, o => o.MapFrom(m => m.Player.PlayerId))
                .ForMember(m => m.Name, o => o.MapFrom(m => m.Player.Name))
                .ForMember(m => m.Position, o => o.MapFrom(m => m.Player.Position))
                .ForMember(m => m.Team, o => o.MapFrom(m => m.PlayerTeam.Team));
            CreateMap<WeeklyProjectionError, WeeklyProjectionErrorModel>();
            CreateMap<TeamMap, TeamMapModel>();
            CreateMap<GameResult, GameResultModel>();
            CreateMap<TeamRecord, TeamRecordModel>()
                .ForMember(t => t.TeamId, o => o.MapFrom(t => t.TeamMap.TeamId));
            CreateMap<ScheduleDetails, ScheduleDetailsModel>();
            CreateMap<FantasyPerformance, FantasyPerformanceModel>();
            CreateMap<Tunings, TuningsModel>().ReverseMap();
            CreateMap<WeeklyTunings, WeeklyTuningsModel>().ReverseMap();
            CreateMap<Schedule, ScheduleModel>();
            CreateMap<Player, PlayerDataModel>();
            CreateMap<TrendingPlayer, TrendingPlayerModel>()
                .ForMember(t => t.PlayerId, o => o.MapFrom(t => t.Player.PlayerId))
                .ForMember(t => t.Name, o => o.MapFrom(t => t.Player.Name))
                .ForMember(t => t.Position, o => o.MapFrom(t => t.Player.Position));
            CreateMap<WaiverWireCandidate, WaiverWireCandidateModel>()
                .ForMember(w => w.Name, o => o.MapFrom(w => w.Player.Name))
                .ForMember(w => w.PlayerId, o => o.MapFrom(w => w.Player.PlayerId))
                .ForMember(w => w.Position, o => o.MapFrom(w => w.Player.Position))
                .ForMember(w => w.Team, o => o.MapFrom(w => w.PlayerTeam != null ? w.PlayerTeam.Team : string.Empty));
            CreateMap<Player, SimplePlayerModel>();
            CreateMap<SnapCount, SnapCountModel>();
            CreateMap<MatchupProjections, MatchupProjectionsModel>()
                .ForMember(m => m.TeamProjections, o => o.Ignore());
            CreateMap<WeeklyFantasy, TopOpponentsModel>()
                .ForMember(w => w.Team, o => o.Ignore())
                .ForMember(w => w.AverageFantasy, o => o.Ignore())
                .ForMember(w => w.TopOpponentTeamDescription, o => o.Ignore());
            CreateMap<PlayerInjury, PlayerInjuryModel>()
                .ForMember(p => p.Name, o => o.MapFrom(p => p.Player.Name))
                .ForMember(p => p.Position, o => o.MapFrom(p => p.Player.Position));
            CreateMap<InSeasonInjury, InSeasonInjuryModel>().ReverseMap();
            CreateMap<SeasonAdjustments, SeasonAdjustmentsModel>().ReverseMap();
            CreateMap<AdjustmentDescription, AdjustmentDescriptionModel>().ReverseMap();
            CreateMap<WeeklyFantasyTrend, WeeklyFantasyTrendModel>();
            CreateMap<PlayerTeam, PlayerTeamModel>();
            CreateMap<TeamLeagueInformation, TeamLeagueInformationModel>();
            CreateMap<InSeasonTeamChange, InSeasonTeamChangeModel>().ReverseMap();
            CreateMap<QualityStarts, QualityStartsModel>()
                .ForMember(qm => qm.PoorStartPercentage, o => o.MapFrom(q => Math.Round(((double)q.PoorStarts / (double)q.GamesPlayed) * 100, 1)))
                .ForMember(qm => qm.GoodStartPercentage, o => o.MapFrom(q => Math.Round(((double)q.GoodStarts / (double)q.GamesPlayed) * 100, 1)))
                .ForMember(qm => qm.GreatStartPercentage, o => o.MapFrom(q => Math.Round(((double)q.GreatStarts / (double)q.GamesPlayed) * 100, 1)))
                .ForMember(qm => qm.QualityCount, o => o.MapFrom(q => q.GoodStarts + q.GreatStarts))
                .ForMember(qm => qm.QualityPercentage, o => o.MapFrom(q => Math.Round(((double)(q.GoodStarts + q.GreatStarts) / (double)q.GamesPlayed) * 100, 1)));
            CreateMap<TeamDepthChart, TeamDepthChartModel>();
        }

    }
}
