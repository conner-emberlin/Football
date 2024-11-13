using Dapper;
using Football.Players.Interfaces;
using Football.Players.Models;
using System.Data;

namespace Football.Players.Repository
{
    public class TeamsRepository(IDbConnection dbConnection) : ITeamsRepository
    {
        public async Task<TeamLeagueInformation> GetTeamLeagueInformation(int teamId)
        {
            var query = $@"SELECT * FROM [dbo].TeamLeagueInformation WHERE TeamId = @teamId";
            return (await dbConnection.QueryAsync<TeamLeagueInformation>(query, new { teamId })).First();
        }

        public async Task<IEnumerable<TeamLeagueInformation>> GetTeamsLeagueInformationByDivision(string division, int teamId = 0)
        {
            var query = $@"SELECT * FROM [dbo].TeamLeagueInformation 
                            WHERE [Division] = @division";
            if (teamId > 0) query += " AND [TeamId] <> @teamId";

            return await dbConnection.QueryAsync<TeamLeagueInformation>(query, new { division, teamId });
        }

        public async Task<IEnumerable<TeamLeagueInformation>> GetTeamLeagueInformationByConference(string conference)
        {
            var query = $@"SELECT * FROM [dbo].TeamLeagueInformation 
                            WHERE [Conference] = @conference";
            return await dbConnection.QueryAsync<TeamLeagueInformation>(query, new { conference });
        }
    }
}
