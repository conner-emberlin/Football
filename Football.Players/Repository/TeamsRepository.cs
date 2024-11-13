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

        public async Task<int> GetTeamId(string teamName)
        {
            var query = $@"SELECT [TeamId] FROM [dbo].TeamMap
                        WHERE [Team] = @teamName";
            return (await dbConnection.QueryAsync<int>(query, new { teamName })).FirstOrDefault();
        }
        public async Task<int> GetTeamId(int playerId)
        {
            var query = $@"SELECT [TeamId] FROM [dbo].TeamMap
                        WHERE [PlayerId] = @playerId";
            return (await dbConnection.QueryAsync<int>(query, new { playerId })).FirstOrDefault();
        }
        public async Task<int> GetTeamIdFromDescription(string teamDescription)
        {
            var query = $@"SELECT [TeamId] FROM [dbo].TeamMap
                        WHERE [TeamDescription] 
                        LIKE '%' + @teamDescription + '%'";
            return (await dbConnection.QueryAsync<int>(query, new { teamDescription })).FirstOrDefault();
        }
        public async Task<List<TeamMap>> GetAllTeams()
        {
            var query = $@"SELECT [TeamId], [Team], [TeamDescription], [PlayerId]
                        FROM [dbo].TeamMap
                        ";
            return (await dbConnection.QueryAsync<TeamMap>(query)).ToList();
        }
        public async Task<TeamMap> GetTeam(int teamId)
        {
            var query = $@"SELECT [TeamId], [Team], [TeamDescription], [PlayerId]
                        FROM [dbo].TeamMap
                        WHERE [TeamId] = @teamId";
            return (await dbConnection.QueryAsync<TeamMap>(query, new { teamId })).First();

        }
        public async Task<PlayerTeam?> GetPlayerTeam(int season, int playerId)
        {
            var query = $@"SELECT *
                        FROM [dbo].PlayerTeam
                        WHERE [Season] = @season
                            AND [PlayerId] = @playerId";

            return (await dbConnection.QueryAsync<PlayerTeam>(query, new { season, playerId })).FirstOrDefault();
        }

        public async Task<IEnumerable<PlayerTeam>> GetPlayerTeams(int season, IEnumerable<int> playerIds)
        {
            var query = $@"SELECT * FROM [dbo].PlayerTeam
                            WHERE [PlayerId] IN @playerIds
                                AND [Season] = @season";
            return await dbConnection.QueryAsync<PlayerTeam>(query, new { playerIds, season });
        }
        public async Task<List<PlayerTeam>> GetPlayersByTeam(string team, int season)
        {
            var query = $@"SELECT [PlayerId], [Name], [Season], [Team]
                        FROM [dbo].PlayerTeam
                        WHERE [Team] = @team
                            AND [Season] = @season";

            return (await dbConnection.QueryAsync<PlayerTeam>(query, new { team, season })).ToList();
        }

        public async Task<IEnumerable<PlayerTeam>> GetPlayersByTeamIdAndPosition(int teamId, string position, int season, bool activeOnly = false)
        {
            var newQuery = "";

            var query = $@"SELECT * FROM [dbo].[PlayerTeam] pt 
                            WHERE pt.TeamId = @teamId
                                AND pt.Season = @season
                                AND EXISTS (SELECT 1 FROM [dbo].Players p
                                            WHERE p.PlayerId = pt.PlayerId
                                                AND p.Position = @position";

            if (activeOnly)
            {
                newQuery = query + " AND p.Active = 1)";
            }
            else newQuery = query + ")";

            return await dbConnection.QueryAsync<PlayerTeam>(newQuery, new { teamId, position, season });
        }

        public async Task<List<Schedule>> GetUpcomingGames(int teamId, int season, int currentWeek)
        {
            var query = $@"SELECT [Season], [TeamId], [Team], [Week], [OpposingTeamId], [OpposingTeam]
                        FROM [dbo].Schedule
                        WHERE [TeamId] = @teamId
                            AND [Week] >= @currentWeek
                            AND [Season] = @season";
            return (await dbConnection.QueryAsync<Schedule>(query, new { teamId, season, currentWeek })).ToList();
        }
        public async Task<List<Schedule>> GetTeamGames(int teamId, int season)
        {
            var query = $@"SELECT [Season], [TeamId], [Team], [Week], [OpposingTeamId], [OpposingTeam]
                        FROM [dbo].Schedule
                        WHERE [TeamId] = @teamId
                            AND [Season] = @season";
            return (await dbConnection.QueryAsync<Schedule>(query, new { teamId, season })).ToList();
        }
        public async Task<TeamLocation> GetTeamLocation(int teamId)
        {
            var query = $@"SELECT * FROM [dbo].TeamLocation
                            WHERE [TeamId] = @teamId";
            return (await dbConnection.QueryAsync<TeamLocation>(query, new { teamId })).First();
        }
        public async Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].ScheduleDetails
                            WHERE [Season] = @season
                                AND [Week] = @week";
            return (await dbConnection.QueryAsync<ScheduleDetails>(query, new { season, week })).ToList();
        }

        public async Task<IEnumerable<Schedule>> GetByeWeeks(int season)
        {
            var query = $@"SELECT * FROM [dbo].Schedule WHERE [Season] = @season AND [OpposingTeamId] = 0";
            return await dbConnection.QueryAsync<Schedule>(query, new { season });
        }
        public async Task<IEnumerable<Schedule>> GetWeeklySchedule(int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].Schedule
                            WHERE [Season] = @season
                                AND [Week] = @week";
            return await dbConnection.QueryAsync<Schedule>(query, new { season, week });
        }
    }
}
