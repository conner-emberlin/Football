using Moq;
using Moq.AutoMock;
using Xunit;
using Football.Projections.Services;
using Football.Projections.Models;
using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Tests
{
    public class MatrixCalculatorShould
    {
        private readonly AutoMocker _mock;
        private readonly Mock<ISettingsService> _mockSettingsService;
        private readonly MatrixCalculator _sut;

        public MatrixCalculatorShould()
        {
            _mock = new AutoMocker();
            _mockSettingsService = new Mock<ISettingsService>();
            _sut = new MatrixCalculator(_mockSettingsService.Object);
        }

        [Fact]
        public void RegressorMatrix_HasCorrectDimensionsAndNonZeroEntries()
        {
            WRModelWeek q1 = new() { PlayerId = 1, ProjectedPoints = 100, ReceptionsPerGame = 10, Season = 2023, SnapsPerGame = 50, TargetsPerGame = 15, TouchdownsPerGame = 1, Week = 5, YardsPerGame = 100, YardsPerReception = 10 };
            WRModelWeek q2 = new() { PlayerId = 2, ProjectedPoints = 200, ReceptionsPerGame = 5, Season = 2023, SnapsPerGame = 35, TargetsPerGame = 10, TouchdownsPerGame = 0.5, Week = 5, YardsPerGame = 150, YardsPerReception = 12 };
            WRModelWeek q3 = new() { PlayerId = 3, ProjectedPoints = 300, ReceptionsPerGame = 11, Season = 2023, SnapsPerGame = 50, TargetsPerGame = 15, TouchdownsPerGame = 1, Week = 5, YardsPerGame = 100, YardsPerReception = 10 };
            List<WRModelWeek> model = [q1, q2, q3];
            var wrProperties = (typeof(WRModelWeek).GetProperties())
                                    .Where(p => !p.ToString()!.Contains(Model.PlayerId.ToString())
                                             && !p.ToString()!.Contains(Model.Season.ToString())
                                             && !p.ToString()!.Contains(Model.Week.ToString())).ToList();
            _mockSettingsService.Setup(s => s.GetPropertiesFromModel<WRModelWeek>()).Returns(wrProperties);

            var actual = _sut.RegressorMatrix(model);

            Assert.Equal(model.Count, actual.RowCount);
            Assert.Equal(wrProperties.Count + 1, actual.ColumnCount);
            Assert.Equal(1, actual[0, 0]);
            Assert.True(actual[1, 2] > 0);
        }

        [Fact]
        public void DependentVector_HasCorrectDimensionsAndNonZeroEntries()
        {
            WeeklyFantasy w1 = new() { Season = 2023, Week = 5, Games = 1, Name = "Name1", PlayerId = 1, Position = "WR", FantasyPoints = 20 };
            WeeklyFantasy w2 = new() { Season = 2023, Week = 5, Games = 1, Name = "Name2", PlayerId = 2, Position = "WR", FantasyPoints = 30 };
            WeeklyFantasy w3 = new() { Season = 2023, Week = 5, Games = 1, Name = "Name3", PlayerId = 3, Position = "WR", FantasyPoints = 40 };
            List<WeeklyFantasy> dependents = [w1, w2, w3];
            var properties = typeof(WeeklyFantasy).GetProperties().ToList();
            _mockSettingsService.Setup(s => s.GetPropertiesFromModel<WeeklyFantasy>()).Returns(properties);

            var actual = _sut.DependentVector(dependents, Model.FantasyPoints);

            Assert.Equal(dependents.Count, actual.Count);
            Assert.True(actual[1] > 0);
        }
    }
}
