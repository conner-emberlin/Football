using Football.Fantasy.MockDraft.Models;
using Football.Fantasy.MockDraft.Interfaces;
using Football.Data.Models;
using Dapper;
using System.Data;

namespace Football.Fantasy.MockDraft.Repository
{
    public class MockDraftRepository(IDbConnection dbConnection) : IMockDraftRepository
    {
        public async Task<List<SeasonADP>> GetAvailablePlayers(Mock mock, int season)
        {
            var mockDraftId = mock.MockDraftId;

            var query = $@"SELECT * FROM [dbo].[ADP] a
                        WHERE [Season] = @season
                        AND NOT EXISTS (SELECT * FROM [dbo].[MockDraftResults] m
                                        WHERE a.PlayerId = m.PlayerId
                                              AND m.MockDraftId = @mockDraftId)";
            return (await dbConnection.QueryAsync<SeasonADP>(query, new { season, mockDraftId })).ToList();
        }

        public async Task<int> AddPlayerToTeam(MockDraftResults result)
        {
            var query = $@"INSERT INTO [dbo].[MockDraftResults] (MockDraftId, Round, FantasyTeamId, Pick, PlayerId)
                            VALUES (@MockDraftId, @Round, @FantasyTeamId, @Pick, @PlayerId)";
            return await dbConnection.ExecuteAsync(query, result);
        }

        public async Task<int> CreateMockDraft(int teamCount, int userPosition)
        {
            var query = $@"INSERT INTO [dbo].[MockDraftLookup] (TeamCount, UserPosition)
                            OUTPUT INSERTED.MockDraftId
                            VALUES (@teamCount, @userPosition)";
            return await dbConnection.QuerySingleAsync<int>(query, new { teamCount, userPosition });
        }
        public async Task<int> CreateFantasyTeam(int mockDraftId, int draftPosition, string teamName)
        {
            var query = $@"INSERT INTO [dbo].[MockDraftFantasyTeams] (MockDraftId, DraftPosition, FantasyTeamName)
                            OUTPUT INSERTED.FantasyTeamId
                            VALUES (@mockDraftId, @draftPosition, @teamName)";
            return await dbConnection.QuerySingleAsync<int>(query, new { mockDraftId, draftPosition, teamName });
        }
    }
}
