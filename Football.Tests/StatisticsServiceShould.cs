using Moq;
using Moq.AutoMock;
using AutoMapper;
using Xunit;
using Football.Players.Interfaces;
using Football.Players.Services;
using Football.Players.Models;
using Football.Enums;


namespace Football.Tests
{
    public class StatisticsServiceShould
    {
        private readonly AutoMocker _mock;
        private readonly StatisticsService _sut;

        private readonly int _playerId = 1;
        private readonly string _team = "TM";
        private readonly int _season = 2023;
        private readonly int _teamId = 10;
        private readonly TeamMap _teamMap;

        private readonly Mock<IStatisticsRepository> _mockStatisticsRepository;
        private readonly Mock<ISettingsService> _mockSettingsService;
        private readonly Mock<IPlayersService> _mockPlayersService;

        public StatisticsServiceShould()
        {
            _mock = new AutoMocker();
            _mockStatisticsRepository = _mock.GetMock<IStatisticsRepository>();
            _mockSettingsService = _mock.GetMock<ISettingsService>();
            _mockPlayersService = _mock.GetMock<IPlayersService>();

            _teamMap = new() { PlayerId = _playerId, Team = _team, TeamDescription = "Description", TeamId = _teamId };

            _mockPlayersService.Setup(ps => ps.GetCurrentWeek(_season)).ReturnsAsync(1);
            _mockPlayersService.Setup(ps => ps.GetAllTeams()).ReturnsAsync([_teamMap]);

            _sut = new StatisticsService(_mockStatisticsRepository.Object, _mockPlayersService.Object, _mockSettingsService.Object);
        }

        [Fact]
        public async Task GetWeeklyData_WithTeam_FiltersTeamChangeWeeks()
        {
            WeeklyDataQB week1 = new() { Week = 1 };
            WeeklyDataQB week2 = new() { Week = 2 };
            WeeklyDataQB week3 = new() { Week = 3 };
            List<WeeklyDataQB> weeks = [week1, week2, week3];
            InSeasonTeamChange teamChange = new() { NewTeam = _team, PlayerId = _playerId, WeekEffective = 2 };
            List<InSeasonTeamChange> teamChanges = [teamChange];

            _mockSettingsService.Setup(s => s.GetValueFromModel(week1, Model.Week)).Returns(1);
            _mockSettingsService.Setup(s => s.GetValueFromModel(week2, Model.Week)).Returns(2);
            _mockSettingsService.Setup(s => s.GetValueFromModel(week3, Model.Week)).Returns(3);

            _mockPlayersService.Setup(ps => ps.GetInSeasonTeamChanges()).ReturnsAsync(teamChanges);
            _mockStatisticsRepository.Setup(sr => sr.GetWeeklyData<WeeklyDataQB>(Position.QB, It.IsAny<int>())).ReturnsAsync(weeks);

            var actual = await _sut.GetWeeklyData<WeeklyDataQB>(Position.QB, _playerId, _team);
            var observedMinWeek = actual.Min(a => a.Week);
            Assert.Equal(2, observedMinWeek);
        }

        [Fact]
        public async Task GetTeamRecords_NoGameResults_ReturnsZeros()
        {
            _mockStatisticsRepository.Setup(sr => sr.GetGameResults(_season)).ReturnsAsync([]);
            
            var actual = await _sut.GetTeamRecords(_season);
            var observed = actual.FirstOrDefault();

            Assert.NotNull(observed);
            Assert.Equal(0, observed.Wins);
            Assert.Equal(0, observed.Losses);
            Assert.Equal(0, observed.Ties);
        }

        [Fact]
        public async Task GetTeamRecords_CalculatesTies()
        {
            var gameResult = new GameResult { HomeTeamId = _teamId, AwayTeamId = 2, LoserPoints = 10, WinnerPoints = 10, Season = _season };

            _mockStatisticsRepository.Setup(sr => sr.GetGameResults(_season)).ReturnsAsync([gameResult]);

            var actual = await _sut.GetTeamRecords(_season);
            var observed = actual.FirstOrDefault();

            Assert.NotNull(observed);
            Assert.Equal(0, observed.Wins);
            Assert.Equal(0, observed.Losses);
            Assert.Equal(1, observed.Ties);
        }
    }
}
