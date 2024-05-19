using Moq;
using Moq.AutoMock;
using AutoMapper;
using Xunit;
using Football.Models;
using Football.Fantasy;
using Football.Fantasy.Services;
using Microsoft.Extensions.Options;
using Football.Players.Models;


namespace Football.Tests
{
    public class FantasyCalculatorShould
    {       
        private readonly AutoMocker _mock;
        private readonly IMapper _mapper;
        private readonly Mock<IOptionsMonitor<FantasyScoring>> _mockScoring;

        private readonly FantasyScoring _scoring;
        private readonly FantasyCalculator _sut;
        public FantasyCalculatorShould()
        {
            _mock = new AutoMocker();
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new AutomapperProfile())));
            _scoring = new FantasyScoring
            {
                PointsPerYard = 0.1,
                PointsPerReception = 1,
                PointsPerTouchdown = 6,
                PointsPerFumble = 2,
                PointsPerPassingYard = 0.04,
                PointsPerPassingTouchdown = 6,
                PointsPerInterception = 2
            };
            _mockScoring = _mock.GetMock<IOptionsMonitor<FantasyScoring>>();
            _mockScoring.Setup(s => s.CurrentValue).Returns(_scoring);
            _sut = new FantasyCalculator(_mockScoring.Object, _mapper);
        }

        [Fact]
        public void CalculateFantasy_QB_Weekly()
        {
            var data = new WeeklyDataQB { Fumbles = 1, Int = 1, RushingTD = 1, RushingYards = 100, Yards = 200, TD = 3 };
            
            var actual = _sut.CalculateFantasy(data);

            Assert.Equal(38, actual.FantasyPoints);
        }

        [Fact]
        public void CalculateFantasy_QB_Season()
        {
            var data = new SeasonDataQB { Fumbles = 1, Int = 1, RushingTD = 1, RushingYards = 100, Yards = 200, TD = 3 };

            var actual = _sut.CalculateFantasy(data);

            Assert.Equal(38, actual.FantasyPoints);
        }

        [Fact]
        public void CalculateFantasy_RB_Weekly()
        {
            var data = new WeeklyDataRB { Fumbles = 1, ReceivingTD = 1, Receptions = 5, RushingTD = 2, Yards = 100 };

            var actual = _sut.CalculateFantasy(data);

            Assert.Equal(31, actual.FantasyPoints);
        }

        [Fact]
        public void CalculateFantasy_RB_Season()
        {
            var data = new SeasonDataRB { Fumbles = 1, ReceivingTD = 1, Receptions = 5, RushingTD = 2, Yards = 100 };

            var actual = _sut.CalculateFantasy(data);

            Assert.Equal(31, actual.FantasyPoints);
        }

        [Fact]
        public void CalculateFantasy_WR_Weekly()
        {
            var data = new WeeklyDataWR { Fumbles = 1, Receptions = 10, RushingTD = 0, RushingYds = 0, TD = 1, Yards = 100 };

            var actual = _sut.CalculateFantasy(data);

            Assert.Equal(24, actual.FantasyPoints);
        }

        [Fact]
        public void CalculateFantasy_WR_Season()
        {
            var data = new SeasonDataWR { Fumbles = 1, Receptions = 10, RushingTD = 0, RushingYds = 0, TD = 1, Yards = 100 };

            var actual = _sut.CalculateFantasy(data);

            Assert.Equal(24, actual.FantasyPoints);
        }

        [Fact]
        public void CalculateFantasy_TE_Weekly()
        {
            var data = new WeeklyDataTE { Fumbles = 1, Receptions = 10, RushingTD = 0, RushingYds = 0, TD = 1, Yards = 100 };

            var actual = _sut.CalculateFantasy(data);

            Assert.Equal(24, actual.FantasyPoints);
        }

        [Fact]
        public void CalculateFantasy_TE_Season()
        {
            var data = new SeasonDataTE { Fumbles = 1, Receptions = 10, RushingTD = 0, RushingYds = 0, TD = 1, Yards = 100 };

            var actual = _sut.CalculateFantasy(data);

            Assert.Equal(24, actual.FantasyPoints);
        }
    }
}
