using Moq;
using Moq.AutoMock;
using Xunit;
using Football.Enums;
using Football.Models;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Fantasy.Interfaces;
using Microsoft.Extensions.Options;
using Football.Players.Interfaces;
using Football.Projections.Services;

namespace Football.Tests
{
    public class ProjectionAnalysisServiceShould
    {
        private readonly AutoMocker _mock;
        private readonly Season _season;
        private readonly Starters _starters;

        private readonly Mock<IProjectionService<SeasonProjection>> _mockSeasonProjectionService;
        private readonly Mock<IProjectionService<WeekProjection>> _mockWeekProjectionService;
        private readonly Mock<IFantasyDataService> _mockFantasyDataService;
        private readonly Mock<IPlayersService> _mockPlayersService;
        private readonly Mock<IOptionsMonitor<Season>> _mockSeason;
        private readonly Mock<IOptionsMonitor<Starters>> _mockStarters;

        private readonly ProjectionAnalysisService _sut;

        public ProjectionAnalysisServiceShould()
        {
            _mock = new AutoMocker();
            _season = new Season() { CurrentSeason = 2023, Games = 17, Weeks = 18 };
            _starters = new Starters() { QBStarters = 24, RBStarters = 24, WRStarters = 24, TEStarters = 12 };

            _mockSeasonProjectionService = _mock.GetMock<IProjectionService<SeasonProjection>>();
            _mockWeekProjectionService = _mock.GetMock<IProjectionService<WeekProjection>>();
            _mockFantasyDataService = _mock.GetMock<IFantasyDataService>();
            _mockPlayersService = _mock.GetMock<IPlayersService>();
            _mockSeason = _mock.GetMock<IOptionsMonitor<Season>>();
            _mockStarters = _mock.GetMock<IOptionsMonitor<Starters>>();

            _mockSeason.Setup(s => s.CurrentValue).Returns(_season);
            _mockStarters.Setup(s => s.CurrentValue).Returns(_starters);

            _sut = new ProjectionAnalysisService(_mockFantasyDataService.Object, _mockSeason.Object, _mockSeasonProjectionService.Object, _mockWeekProjectionService.Object, _mockStarters.Object, _mockPlayersService.Object);
        }

        [Fact]
        public async Task SeasonFlexRankings_AreOrderedByVorp()
        {
            var qbProjection1 = new SeasonProjection { Name = "QB Name1", PlayerId = 1, Position = "QB", ProjectedPoints = 100, Season = _season.CurrentSeason };
            var qbProjection2 = new SeasonProjection { Name = "QB Name2", PlayerId = 2, Position = "QB", ProjectedPoints = 110, Season = _season.CurrentSeason };
            var teProjection1 = new SeasonProjection { Name = "TE Name1", PlayerId = 4, Position = "TE", ProjectedPoints = 100, Season = _season.CurrentSeason };
            var teProjection2 = new SeasonProjection { Name = "TE Name2", PlayerId = 5, Position = "TE", ProjectedPoints = 50, Season = _season.CurrentSeason };

            _mockSeasonProjectionService.Setup(s => s.GetProjections(Position.QB)).ReturnsAsync([qbProjection1, qbProjection2]);
            _mockSeasonProjectionService.Setup(s => s.GetProjections(Position.WR)).ReturnsAsync([]);
            _mockSeasonProjectionService.Setup(s => s.GetProjections(Position.RB)).ReturnsAsync([]);
            _mockSeasonProjectionService.Setup(s => s.GetProjections(Position.TE)).ReturnsAsync([teProjection1, teProjection2]);

            var actual = await _sut.SeasonFlexRankings();

            Assert.True(actual.ElementAt(0).PlayerId == 4 && actual.ElementAt(1).PlayerId == 2);
        }

    }
}
