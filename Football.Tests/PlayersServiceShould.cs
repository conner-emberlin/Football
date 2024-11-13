using Moq;
using Moq.AutoMock;
using AutoMapper;
using Xunit;
using Football.Players;
using Football.Players.Services;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Football.Enums;

namespace Football.Tests
{
    public class PlayersServiceShould
    {
        private readonly AutoMocker _mock;
        private readonly PlayersService _sut;
        private readonly Season _season;
        private readonly TeamMap _teamMap;
        private readonly Player _playerDST;
        private readonly IMapper _mapper;

        private readonly int _playerId = 1;
        private readonly string _team = "TM";
        private readonly int _teamId = 10;
        private readonly Position _position = Position.QB;

        private readonly Mock<IPlayersRepository> _mockPlayersRepository;
        private readonly Mock<ISleeperLeagueService> _mockSleeperLeagueService;
        private readonly Mock<IOptionsMonitor<Season>> _mockSeason;
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly Mock<ISettingsService> _mockSettingsService;
        private readonly Mock<ILogger> _mockLogger;

        public PlayersServiceShould()
        {
            _mock = new AutoMocker();
            _season = new Season { CurrentSeason = 2023 };
            _teamMap = new() { TeamId = _teamId, Team = _team, TeamDescription = "Team Name", PlayerId = _playerId };
            _playerDST = new() { Name = "Team Name", Active = 1, PlayerId = _playerId, Position = "DST" };
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new AutomapperProfile())));

            _mockPlayersRepository = _mock.GetMock<IPlayersRepository>();
            _mockSeason = _mock.GetMock<IOptionsMonitor<Season>>();
            _mockMemoryCache = _mock.GetMock<IMemoryCache>();
            _mockSettingsService = _mock.GetMock<ISettingsService>();
            _mockLogger = _mock.GetMock<ILogger>();
            _mockSleeperLeagueService = _mock.GetMock<ISleeperLeagueService>();

            _mockSeason.Setup(s => s.CurrentValue).Returns(_season);
            _mockPlayersRepository.Setup(pr => pr.GetPlayer(_playerId)).ReturnsAsync(_playerDST);


            _sut = new PlayersService(_mockPlayersRepository.Object, _mockMemoryCache.Object, _mockSeason.Object, _mockSettingsService.Object, _mapper, _mockLogger.Object, _mockSleeperLeagueService.Object);
        }

        [Fact]
        public async Task GetPlayerTeam_DST_ReturnsNameAsDescriptionAndIsNotNull()
        {
            var actual = await _sut.GetPlayerTeam(_season.CurrentSeason, _playerId);
            Assert.NotNull(actual);
            Assert.Equal(_playerDST.Name, actual.Name);
        }

        [Fact] 
        public async Task GetPlayersByTeam_IncludesFormerPlayers()
        {
            List<PlayerTeam> playerTeams = [];
            List<InSeasonTeamChange> teamChanges = [];
            var formerPlayer = new Player { Name = "Former", PlayerId = 3, Active = 1, Position = "QB" };
            var inSeasonTeamChange = new InSeasonTeamChange { PlayerId = 3, PreviousTeam = _team, NewTeam = "NEW", Season = _season.CurrentSeason, WeekEffective = 10 };
            teamChanges.Add(inSeasonTeamChange);

            _mockPlayersRepository.Setup(pr => pr.GetPlayersByTeam(_teamMap.Team, It.IsAny<int>())).ReturnsAsync(playerTeams);
            _mockPlayersRepository.Setup(ps => ps.GetInSeasonTeamChanges(It.IsAny<int>())).ReturnsAsync(teamChanges);
            _mockPlayersRepository.Setup(pr => pr.GetPlayer(inSeasonTeamChange.PlayerId)).ReturnsAsync(formerPlayer);
            
            var expected = new PlayerTeam { Name = "Former", PlayerId = 3, Season = _season.CurrentSeason, Team = _team };
            var actual = await _sut.GetPlayersByTeam(_team);

            Assert.Contains(actual, pt => pt.Name == expected.Name && pt.Team == expected.Team && pt.PlayerId == expected.PlayerId);
        }

        [Fact]
        public async Task GetPlayersByTeam_IncludesTeamDST()
        {
            List<PlayerTeam> playerTeams = [];
            List<InSeasonTeamChange> teamChanges = [];
            _mockPlayersRepository.Setup(pr => pr.GetPlayersByTeam(_teamMap.Team, It.IsAny<int>())).ReturnsAsync(playerTeams);
            _mockPlayersRepository.Setup(ps => ps.GetInSeasonTeamChanges(It.IsAny<int>())).ReturnsAsync(teamChanges);

            var actual = await _sut.GetPlayersByTeam(_team);
            Assert.Contains(actual, pt => pt.PlayerId == _playerId && pt.Team == _team);
        }

        [Fact]
        public async Task RetrievePlayer_PlayerDoesNotExist_PlayerCreated()
        {
            var name = "Player Name";
            _mockPlayersRepository.Setup(pr => pr.GetPlayerByName(name)).ReturnsAsync((Player?)null);
            _mockPlayersRepository.Setup(pr => pr.CreatePlayer(name, 1, _position.ToString())).ReturnsAsync(_playerId);

            var actual = await _sut.RetrievePlayer(name, _position);

            Assert.True(actual.PlayerId == _playerId && actual.Position == _position.ToString() && actual.Name == name);
        }

        [Fact]
        public async Task RetrievePlayer_PlayerExistsAndInactive_ActivatesPlayerWhenTrue()
        {
            var name = "Player Name";
            var player = new Player { Name = name, Active = 0, Position = _position.ToString(), PlayerId = _playerId };
            _mockPlayersRepository.Setup(pr => pr.GetPlayerByName(name)).ReturnsAsync(player);
            _mockPlayersRepository.Setup(pr => pr.ActivatePlayer(_playerId)).ReturnsAsync(1);

            var actual = await _sut.RetrievePlayer(name, _position, true);

            Assert.Equal(1, actual.Active);
        }
    }
}
