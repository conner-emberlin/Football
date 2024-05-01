using Football.Fantasy.MockDraft.Models;
using Football.Fantasy.MockDraft.Interfaces;
using Football.Enums;
using Football.Models;
using Microsoft.Extensions.Options;
using Football.Data.Models;

namespace Football.Fantasy.MockDraft.Services
{
    public class MockDraftService(IMockDraftRepository mockDraftRepository, IOptionsMonitor<MockDraftSettings> settings, IOptionsMonitor<Season> season) : IMockDraftService
    {
        private readonly MockDraftSettings _settings = settings.CurrentValue;
        private readonly Season _season = season.CurrentValue;

        public async Task<List<Position>> GetTeamNeeds(FantasyTeam team)
        {
            return await Task.Run(() =>
            {
                List<Position> needs = [];
                var qbCount = team.Players.Count(p => p.Position == Position.QB.ToString());
                var rbCount = team.Players.Count(p => p.Position == Position.RB.ToString());
                var wrCount = team.Players.Count(p => p.Position == Position.WR.ToString());
                var teCount = team.Players.Count(p => p.Position == Position.TE.ToString());

                if (qbCount < _settings.QBStarters) needs.Add(Position.QB);
                if (rbCount < _settings.RBStarters) needs.Add(Position.RB);
                if (wrCount < _settings.WRStarters) needs.Add(Position.WR);
                if (teCount < _settings.TEStarters) needs.Add(Position.TE);
                return needs;
            });
        }

        public async Task<List<SeasonADP>> GetPlayerChoices(List<Position> teamNeeds, List<SeasonADP> availablePlayers)
        {
            var t = await Task.Run(() =>
            {
                if (teamNeeds.Count > 0)
                {
                    List<SeasonADP> playerChoice = [];
                    foreach (var need in teamNeeds)
                    {
                        var tempP = availablePlayers.Where(p => p.Position == need.ToString()).Take(3).ToList();
                        foreach (var t in tempP) playerChoice.Add(t);
                    }
                    return playerChoice;
                }
                return availablePlayers.Take(5).ToList();
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
        public async Task<List<SeasonADP>> GetAvailablePlayers(Mock mock) => await mockDraftRepository.GetAvailablePlayers(mock, _season.CurrentSeason);
        public async Task<int> AddPlayerToTeam(MockDraftResults result) => await mockDraftRepository.AddPlayerToTeam(result);
        public async Task<Mock> CreateMockDraft(MockDraftParameters param)
        {
            var mockDraftId = await mockDraftRepository.CreateMockDraft(param.TeamCount, param.UserPosition);
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
        public async Task<int> CreateFantasyTeam(int mockDraftId, int draftPosition, string teamName) => await mockDraftRepository.CreateFantasyTeam(mockDraftId, draftPosition, teamName);
    }
}
