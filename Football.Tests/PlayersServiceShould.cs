using Moq;
using Moq.AutoMock;
using Football.Players.Services;
using Football.Players.Interfaces;
using Football.Players.Models;

namespace Football.Tests
{
    public class PlayersServiceShould
    {
        private readonly AutoMocker _mock;
        private readonly PlayersService _sut;
        private readonly Mock<IPlayersRepository> _mockPlayersRepository;

        private readonly int _season = 2023;

        public PlayersServiceShould()
        {
            _mock = new AutoMocker();
            _mockPlayersRepository = _mock.GetMock<IPlayersRepository>();
            _sut = _mock.CreateInstance<PlayersService>();
        }

        [Fact]
        public async Task GetPlayerTeam_DST_ReturnsNameAsDescriptionAndIsNotNull()
        {
            //Arrange
            var player = new Player { Name = "Team Name", Active = 1, PlayerId = 1, Position = "DST" };
            _mockPlayersRepository.Setup(pr => pr.GetPlayer(1)).ReturnsAsync(player);
            _mockPlayersRepository.Setup(ps => ps.GetTeamId(1)).ReturnsAsync(10);
            _mockPlayersRepository.Setup(ps => ps.GetTeam(10)).ReturnsAsync(new TeamMap { TeamId = 10, Team = "TM", TeamDescription = "Team Name", PlayerId = 1 });

            //Act
            var actual = await _sut.GetPlayerTeam(_season, 1);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Team Name", actual.Name);
        }
    }
}
