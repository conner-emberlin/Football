﻿using Moq;
using Moq.AutoMock;
using Xunit;
using Serilog;
using Football.Players.Interfaces;
using Football.Fantasy.Interfaces;
using Football.Models;
using Microsoft.Extensions.Options;
using Football.Projections.Models;
using Football.Players.Models;
using Football.Enums;
using Football.Projections.Services;
using Football.Fantasy.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Football.Tests
{
    public class AdjustmentServiceShould
    {
        private readonly int _playerId = 1;
        private readonly Position _position = Position.QB;
        private readonly string _team = "TM";
        private readonly int _teamId = 10;
        private readonly int _week = 11;
        private readonly int _projectedPoints = 100;
        private readonly MatchupRanking _m1;
        private readonly MatchupRanking _m2;
        private readonly MatchupRanking _m3;
        private readonly TeamMap _teamMap;
        private readonly int _tm1 = 100;
        private readonly int _tm2 = 200;
        private readonly int _tm3 = 300;
        private readonly PlayerTeam _playerTeam;

        private readonly AutoMocker _mock;
        private readonly Season _season;
        private readonly Tunings _tunings;
        private readonly WeeklyTunings _weeklyTunings;
        private readonly AdjustmentService _sut;

        private readonly Mock<IPlayersService> _mockPlayersService;
        private readonly Mock<IMatchupAnalysisService> _mockMatchupAnalysisService;
        private readonly Mock<IStatisticsService> _mockStatisticsService;
        private readonly Mock<IFantasyAnalysisService> _mockFantasyAnalysisService;
        private readonly Mock<IFantasyDataService> _mockFantasyDataService;
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly Mock<IOptionsMonitor<Season>> _mockSeason;
        private readonly Mock<IOptionsMonitor<Tunings>> _mockTunings;
        private readonly Mock<IOptionsMonitor<WeeklyTunings>> _mockWeeklyTunings;
        

        public AdjustmentServiceShould()
        {
            _mock = new AutoMocker();
            _season = new Season { CurrentSeason = 2023 };
            _tunings = new Tunings { RBFloor = 160, LeadRBFactor = 2.4, Weight = 0.63, SecondYearWRLeap = 1.04, SecondYearQBLeap = 1.04, SecondYearRBLeap = 1.04, NewQBFloor = 0.08, NewQBCeiling = 1.20 };
            _weeklyTunings = new WeeklyTunings { RecentWeekWeight = 0.55, ProjectionWeight = 0.75, TamperedMin = 0.8, TamperedMax = 1.2 };
            _teamMap = new() { Team = _team, TeamId = _teamId, PlayerId = 20, TeamDescription = "Team" };

            _m1 = new() { TeamId = _tm1, GamesPlayed = 10, AvgPointsAllowed = 10, PointsAllowed = 100, Position = _position.ToString() };
            _m2 = new() { TeamId = _tm2, GamesPlayed = 10, AvgPointsAllowed = 20, PointsAllowed = 200, Position = _position.ToString() };
            _m3 = new() { TeamId = _tm3, GamesPlayed = 10, AvgPointsAllowed = 30, PointsAllowed = 300, Position = _position.ToString() };
            _playerTeam = new() { Name = "Player", PlayerId = _playerId, Season = _season.CurrentSeason, Team = _team, TeamId = _teamId };

            _mockPlayersService = _mock.GetMock<IPlayersService>();
            _mockMatchupAnalysisService = _mock.GetMock<IMatchupAnalysisService>();
            _mockStatisticsService = _mock.GetMock<IStatisticsService>();
            _mockFantasyAnalysisService = _mock.GetMock<IFantasyAnalysisService>();
            _mockFantasyDataService = _mock.GetMock<IFantasyDataService>();
            _mockLogger = _mock.GetMock<ILogger>();
            _mockMemoryCache = _mock.GetMock<IMemoryCache>();
            _mockSeason = _mock.GetMock<IOptionsMonitor<Season>>();
            _mockTunings = _mock.GetMock<IOptionsMonitor<Tunings>>();
            _mockWeeklyTunings = _mock.GetMock<IOptionsMonitor<WeeklyTunings>>();

            _mockSeason.Setup(ms => ms.CurrentValue).Returns(_season);
            _mockTunings.Setup(mt => mt.CurrentValue).Returns(_tunings);
            _mockWeeklyTunings.Setup(mw => mw.CurrentValue).Returns(_weeklyTunings);

            _mockPlayersService.Setup(ps => ps.GetPlayerTeams(_season.CurrentSeason, It.IsAny<IEnumerable<int>>())).ReturnsAsync([_playerTeam]);
            _mockPlayersService.Setup(ps => ps.GetTeamId(_team)).ReturnsAsync(_teamId);

            _sut = new AdjustmentService(_mockPlayersService.Object, _mockMatchupAnalysisService.Object, _mockStatisticsService.Object, _mockFantasyAnalysisService.Object, _mockFantasyDataService.Object, _mockSeason.Object, _mockTunings.Object, _mockWeeklyTunings.Object, _mockMemoryCache.Object);
        }

        [Fact]
        public async Task AdjustmentEngine_WeeklyWithInjury_ReturnsZero()
        {
            var weekProjection = new WeekProjection { PlayerId = _playerId, Position = _position.ToString(), Name = "Player", ProjectedPoints = 100, Season = _season.CurrentSeason, Week = 1 };
            List<WeekProjection> projections = [weekProjection];
            var injury = new InSeasonInjury { Season = _season.CurrentSeason, InjuryStartWeek = 1, InjuryEndWeek = 0, PlayerId = _playerId };
            _mockMatchupAnalysisService.Setup(ms => ms.GetPositionalMatchupRankingsFromSQL(_position, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync([]);
            _mockPlayersService.Setup(ps => ps.GetActiveInSeasonInjuries(_season.CurrentSeason)).ReturnsAsync([injury]);

            var actual = await _sut.AdjustmentEngine(projections);
            var observed = actual.FirstOrDefault();

            Assert.NotNull(observed);
            Assert.Equal(0, observed.ProjectedPoints);
        }

        [Fact]
        public async Task AdjustmentEngine_SeasonWithInjuryGames_DeductsFromTotalProjection()
        {
            var seasonProjection = new SeasonProjection { PlayerId = _playerId, Position = _position.ToString(), Name = "Player", ProjectedPoints = 170, Season = _season.CurrentSeason };
            List<SeasonProjection> projections = [seasonProjection];

            _mockPlayersService.Setup(ps => ps.GetPlayerSuspensions(_season.CurrentSeason)).ReturnsAsync([]);
            InjuryConcerns injury = new() { PlayerId = _playerId, Season = _season.CurrentSeason, Games = 5 };
            _mockPlayersService.Setup(ps => ps.GetPlayerInjuries(_season.CurrentSeason)).ReturnsAsync([injury]);
            var expectedPoints = seasonProjection.ProjectedPoints - (double)(seasonProjection.ProjectedPoints / 17 * 5);

            var actual = await _sut.AdjustmentEngine(projections, new Tunings { }, 17);
            var observed = actual.FirstOrDefault();

            Assert.NotNull(observed);
            Assert.Equal(expectedPoints, observed.ProjectedPoints);
        }

        [Fact]
        public async Task AdjustmentEngine_SeasonWithSuspensionGames_DeductsFromTotalProjection()
        {
            var seasonProjection = new SeasonProjection { PlayerId = _playerId, Position = _position.ToString(), Name = "Player", ProjectedPoints = 170, Season = _season.CurrentSeason };
            List<SeasonProjection> projections = [seasonProjection];
            Suspensions suspension = new() { Season = _season.CurrentSeason, PlayerId = _playerId, Length = 5 };
            _mockPlayersService.Setup(ps => ps.GetPlayerSuspensions(_season.CurrentSeason)).ReturnsAsync([suspension]);
            _mockPlayersService.Setup(ps => ps.GetPlayerInjuries(_season.CurrentSeason)).ReturnsAsync([]);
            var expectedPoints = seasonProjection.ProjectedPoints - (double)(seasonProjection.ProjectedPoints / 17 * 5);

            var actual = await _sut.AdjustmentEngine(projections);
            var observed = actual.FirstOrDefault();

            Assert.NotNull(observed);
            Assert.Equal(expectedPoints, observed.ProjectedPoints);
        }

        [Fact]
        public async Task AdjustmentEngine_WeeklyByeWeek_ReturnsZeroProjection()
        {
            var weekProjection = new WeekProjection { PlayerId = _playerId, Position = _position.ToString(), Name = "Player", ProjectedPoints = 100, Season = _season.CurrentSeason, Week = 2 };
            List<WeekProjection> projections = [weekProjection];
            var matchupRanking = new MatchupRanking { TeamId = _teamId, GamesPlayed = 10, AvgPointsAllowed = 20, PointsAllowed = 200, Position = _position.ToString() };
            _mockMatchupAnalysisService.Setup(ma => ma.GetPositionalMatchupRankingsFromSQL(_position, _season.CurrentSeason,2)).ReturnsAsync([matchupRanking]);
            var teamGame = new Schedule { Week = 2, OpposingTeam = "BYE", Season = _season.CurrentSeason, OpposingTeamId = 30, Team = _team, TeamId = _teamId };
            _mockPlayersService.Setup(ps => ps.GetWeeklySchedule(_season.CurrentSeason, 2)).ReturnsAsync([teamGame]);
            _mockPlayersService.Setup(ps => ps.GetActiveInSeasonInjuries(_season.CurrentSeason)).ReturnsAsync([]);

            var actual = await _sut.AdjustmentEngine(projections);
            var observed = actual.FirstOrDefault();

            Assert.NotNull(observed);
            Assert.Equal(0, observed.ProjectedPoints);
        }

        [Fact]
        public async Task AdjustmentEngine_WeeklyMatchupMoreDifficultThanAverage_ReducesProjection()
        {
            var weekProjection = new WeekProjection { PlayerId = _playerId, Position = _position.ToString(), Name = "Player", ProjectedPoints = _projectedPoints, Season = _season.CurrentSeason, Week = _week };
            List<WeekProjection> projections = [weekProjection];
            _mockMatchupAnalysisService.Setup(ma => ma.GetPositionalMatchupRankingsFromSQL(_position, _season.CurrentSeason, _week)).ReturnsAsync([_m1, _m2, _m3]);
            var teamGame = new Schedule { Week = _week, OpposingTeam = "OPP", Season = _season.CurrentSeason, OpposingTeamId = _tm1, Team = _team, TeamId = _teamId };
            _mockPlayersService.Setup(ps => ps.GetWeeklySchedule(_season.CurrentSeason, _week)).ReturnsAsync([teamGame]);
            _mockPlayersService.Setup(ps => ps.GetActiveInSeasonInjuries(_season.CurrentSeason)).ReturnsAsync([]);

            var actual = await _sut.AdjustmentEngine(projections);
            var observed = actual.FirstOrDefault();

            Assert.NotNull(observed);
            Assert.True(observed.ProjectedPoints < _projectedPoints && observed.Week == _week && observed.PlayerId == weekProjection.PlayerId);
        }

        [Fact]
        public async Task AdjustmentEngine_WeeklyMatchupLessDifficultThanAverage_IncreasesProjection()
        {
            var weekProjection = new WeekProjection { PlayerId = _playerId, Position = _position.ToString(), Name = "Player", ProjectedPoints = _projectedPoints, Season = _season.CurrentSeason, Week = _week };
            List<WeekProjection> projections = [weekProjection];
            _mockMatchupAnalysisService.Setup(ma => ma.GetPositionalMatchupRankingsFromSQL(_position, _season.CurrentSeason, _week)).ReturnsAsync([_m1, _m2, _m3]);
            var teamGame = new Schedule { Week = _week, OpposingTeam = "OPP3", Season = _season.CurrentSeason, OpposingTeamId = _tm3, Team = _team, TeamId = _teamId };
            _mockPlayersService.Setup(ps => ps.GetWeeklySchedule(_season.CurrentSeason, _week)).ReturnsAsync([teamGame]);
            _mockPlayersService.Setup(ps => ps.GetActiveInSeasonInjuries(_season.CurrentSeason)).ReturnsAsync([]);

            var actual = await _sut.AdjustmentEngine(projections);
            var observed = actual.FirstOrDefault();

            Assert.NotNull(observed);
            Assert.True(observed.ProjectedPoints > _projectedPoints && observed.Week == _week && observed.PlayerId == weekProjection.PlayerId);
        }
    }
}
