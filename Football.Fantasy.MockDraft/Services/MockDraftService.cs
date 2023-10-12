using Football.Fantasy.MockDraft.Models;
using Football.Players.Models;
using Football.Fantasy.MockDraft.Interfaces;
using Football.Enums;
using Football.Models;
using Microsoft.Extensions.Options;
using Football.Data.Models;

namespace Football.Fantasy.MockDraft.Services
{
    public class MockDraftService : IMockDraftService
    {
        private readonly IMockDraftRepository _mockDraftRepository;
        private readonly MockDraftSettings _settings;
        private readonly Season _season;
        public MockDraftService(IMockDraftRepository mockDraftRepository, IOptionsMonitor<MockDraftSettings> settings, IOptionsMonitor<Season> season) 
        {
            _mockDraftRepository = mockDraftRepository;
            _settings = settings.CurrentValue;
            _season = season.CurrentValue;
        }

        public async Task<List<PositionEnum>> GetTeamNeeds(FantasyTeam team)
        {
            return await Task.Run(() =>
            {
                List<PositionEnum> needs = new();
                var qbCount = team.Players.Where(p => p.Position == PositionEnum.QB.ToString()).Count();
                var rbCount = team.Players.Where(p => p.Position == PositionEnum.RB.ToString()).Count();
                var wrCount = team.Players.Where(p => p.Position == PositionEnum.WR.ToString()).Count();
                var teCount = team.Players.Where(p => p.Position == PositionEnum.TE.ToString()).Count();

                if (qbCount < _settings.QBStarters)
                {
                    needs.Add(PositionEnum.QB);
                }
                if (rbCount < _settings.RBStarters)
                {
                    needs.Add(PositionEnum.RB);
                }
                if (wrCount < _settings.WRStarters)
                {
                    needs.Add(PositionEnum.WR);
                }
                if (teCount < _settings.TEStarters)
                {
                    needs.Add(PositionEnum.TE);
                }
                return needs;
            });
        }

        public async Task<List<SeasonADP>> GetPlayerChoices(List<PositionEnum> teamNeeds, List<SeasonADP> availablePlayers)
        {
            var t = await Task.Run(() =>
            {
                if (teamNeeds.Any())
                {
                    List<SeasonADP> playerChoice = new();
                    foreach (var need in teamNeeds)
                    {
                        var tempP = availablePlayers.Where(p => p.Position == need.ToString()).Take(3).ToList();
                        foreach (var t in tempP)
                        {
                            playerChoice.Add(t);
                        }
                    }
                    return playerChoice;
                }
                else
                {
                    return availablePlayers.Take(5).ToList();
                }
            });
            return t;
        }
        public async Task<SeasonADP> ChoosePlayer(List<SeasonADP> players)
        {
            return await Task.Run(() =>
            {
                Random rnd = new();
                return players.ElementAt(rnd.Next(players.Count - 1));
            });
        }
        public async Task<List<SeasonADP>> GetAvailablePlayers(Mock mock) => await _mockDraftRepository.GetAvailablePlayers(mock, _season.CurrentSeason);
        public async Task<int> AddPlayerToTeam(MockDraftResults result) => await _mockDraftRepository.AddPlayerToTeam(result);
        public async Task<Mock> CreateMockDraft(MockDraftParameters param)
        {
            var mockDraftId = await _mockDraftRepository.CreateMockDraft(param.TeamCount, param.UserPosition);
            if (mockDraftId > 0)
            {
                var teams = await PopulateFantasyTeams(mockDraftId, param.UserPosition, param.TeamCount, param.TeamName);
                return new Mock
                {
                    MockDraftId = mockDraftId,
                    Teams = teams,
                    UserPosition = param.UserPosition
                };
            }
            else return new Mock { };
        }

        public async Task<List<FantasyTeam>> PopulateFantasyTeams(int mockDraftId, int userPosition, int teamCount, string teamName)
        {
            List<FantasyTeam> teams = new();
            teams.Add(new FantasyTeam
            {
                FantasyTeamName = teamName,
                DraftPosition = userPosition
            });
            for (int i = 1; i < userPosition; i++)
            {
                teams.Add(new FantasyTeam
                {
                    FantasyTeamName = "Team" + i.ToString(),
                    DraftPosition = i
                });
            }
            for (int i = userPosition + 1; i <= teamCount; i++)
            {
                teams.Add(new FantasyTeam
                {
                    FantasyTeamName = "Team" + i.ToString(),
                    DraftPosition = i
                });
            }

            foreach (var t in teams)
            {
                t.FantasyTeamId = await CreateFantasyTeam(mockDraftId, t.DraftPosition, t.FantasyTeamName);
            }
            return teams;
        }
        public async Task<int> CreateFantasyTeam(int mockDraftId, int draftPosition, string teamName) => await _mockDraftRepository.CreateFantasyTeam(mockDraftId, draftPosition, teamName);
    }
}
