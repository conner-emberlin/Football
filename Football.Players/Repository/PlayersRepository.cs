using System.Data;
using System.Runtime.CompilerServices;
using Dapper;
using Football.Players.Interfaces;
using Football.Players.Models;

namespace Football.Players.Repository
{
    
    public class PlayersRepository : IPlayersRepository
    {
        private readonly IDbConnection _dbConnection;
        public PlayersRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> GetPlayerId(string name)
        {
            var query = $@"SELECT [PlayerId] FROM [dbo].Players
                            WHERE [Name] = @name
                            ORDER BY [Active] DESC";
            return (await _dbConnection.QueryAsync<int>(query, new { name })).FirstOrDefault();
        }
        public async Task<int> CreatePlayer(Player player)
        {
            var query = $@"INSERT INTO [dbo].Players (Name, Position, Active)
                        VALUES (@Name, @Position, @Active)";
            return await _dbConnection.ExecuteAsync(query, player);
        }
        public async Task<List<Player>> GetAllPlayers()
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Active]
                        FROM [dbo].Players  
                        ";
            return (await _dbConnection.QueryAsync<Player>(query)).ToList();
        }
        public async Task<List<Player>> GetPlayersByPosition(string position)
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Active]
                            FROM [dbo].Players
                            WHERE [Position] = @position";
            return (await _dbConnection.QueryAsync<Player>(query, new { position })).ToList();
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Active]
                        FROM [dbo].Players
                        WHERE [PlayerId] = @playerId";
            return (await _dbConnection.QueryAsync<Player>(query, new { playerId })).First();
        }
        public async Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position)
        {
            var query = $@"SELECT [PlayerId], [TeamDrafted], [Position], [RookieSeason], [DraftPosition], [DeclareAge]
                        FROM [dbo].Rookie
                        WHERE [RookieSeason] < @currentSeason
                        AND [Position] = @position";
            return (await _dbConnection.QueryAsync<Rookie>(query, new {currentSeason, position})).ToList();
        }
        public async Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position)
        {
            var query = $@"SELECT [PlayerId], [TeamDrafted], [Position], [RookieSeason], [DraftPosition], [DeclareAge]
                        FROM [dbo].Rookie
                        WHERE [RookieSeason] = @currentSeason
                        AND [Position] = @position";
            return (await _dbConnection.QueryAsync<Rookie>(query, new { currentSeason, position })).ToList();
        }
        public async Task<int> GetPlayerInjuries(int playerId, int season)
        {
            var query = $@"SELECT [Games] FROM [dbo].InjuryConcerns
                        WHERE [PlayerId] = @playerId
                        AND [Season] = @season";
            return (await _dbConnection.QueryAsync<int>(query, new { playerId, season })).FirstOrDefault();
        }
        public async Task<int> GetPlayerSuspensions(int playerId, int season)
        {
            var query = $@"SELECT [Length] FROM [dbo].Suspensions
                            WHERE [PlayerId] = @playerId
                            AND [Season] = @season";
            return (await _dbConnection.QueryAsync<int>(query, new { playerId, season })).FirstOrDefault();
        }
        public async Task<List<QuarterbackChange>> GetQuarterbackChanges(int season)
        {
            var query = $@"SELECT [PlayerId], [Season], [PreviousQB], [CurrentQB]
                        FROM [dbo].QuarterbackChanges
                        WHERE [Season] = @season";
            return (await _dbConnection.QueryAsync<QuarterbackChange>(query, new {season})).ToList();
        }
        public async Task<double> GetEPA(int playerId, int season)
        {
            var query = $@"SELECT [EPA] FROM [dbo].[EPA] WHERE [PlayerId] = @playerId AND [Season] = @season";
            return (await _dbConnection.QueryAsync<double>(query, new {playerId, season})).FirstOrDefault();
        }
        public async Task<double> GetSeasonProjection(int season, int playerId)
        {
            var query = $@"SELECT [ProjectedPoints]
                        FROM [dbo].SeasonProjections
                        WHERE [PlayerId] = @playerId
                            AND [Season] = @season";
            return (await _dbConnection.QueryAsync<double>(query, new { season, playerId })).FirstOrDefault(); 
        }

        public async Task<double> GetWeeklyProjection(int season, int week, int playerId)
        {
            var query = $@"SELECT [ProjectedPoints]
                            FROM [dbo].WeeklyProjections
                            WHERE [Season] = @season
                                AND [Week] = @week
                                AND [PlayerId] = @playerId";
            return (await _dbConnection.QueryAsync<double>(query, new { season, week, playerId })).FirstOrDefault();
        }
        public async Task<PlayerTeam?> GetPlayerTeam(int season, int playerId)
        {
            var query = $@"SELECT [PlayerId], [Name], [Season], [Team]
                        FROM [dbo].PlayerTeam
                        WHERE [Season] = @season
                            AND [PlayerId] = @playerId";

            return (await _dbConnection.QueryAsync<PlayerTeam>(query, new { season, playerId })).FirstOrDefault();
        }
        public async Task<List<PlayerTeam>> GetPlayersByTeam(string team, int season)
        {
            var query = $@"SELECT [PlayerId], [Name], [Season], [Team]
                        FROM [dbo].PlayerTeam
                        WHERE [Team] = @team
                            AND [Season] = @season";

            return (await _dbConnection.QueryAsync<PlayerTeam>(query, new { team , season})).ToList();
        }
        public async Task<int> GetTeamId(string teamName)
        {
            var query = $@"SELECT [TeamId] FROM [dbo].TeamMap
                        WHERE [Team] = @teamName";
            return (await _dbConnection.QueryAsync<int>(query, new { teamName })).FirstOrDefault();
        }
        public async Task<int> GetTeamId(int playerId)
        {
            var query = $@"SELECT [TeamId] FROM [dbo].TeamMap
                        WHERE [PlayerId] = @playerId";
            return (await _dbConnection.QueryAsync<int>(query, new { playerId })).FirstOrDefault();
        }
        public async Task<int> GetTeamIdFromDescription(string teamDescription)
        {
            var query = $@"SELECT [TeamId] FROM [dbo].TeamMap
                        WHERE [TeamDescription] 
                        LIKE '%' + @teamDescription + '%'";
            return (await _dbConnection.QueryAsync<int>(query, new {teamDescription})).FirstOrDefault();
        }
        public async Task<List<TeamMap>> GetAllTeams()
        {
            var query = $@"SELECT [TeamId], [Team], [TeamDescription], [PlayerId]
                        FROM [dbo].TeamMap
                        ";
            return (await _dbConnection.QueryAsync<TeamMap>(query)).ToList();
        }
        public async Task<TeamMap> GetTeam(int teamId)
        {
            var query = $@"SELECT [TeamId], [Team], [TeamDescription], [PlayerId]
                        FROM [dbo].TeamMap
                        WHERE [TeamId] = @teamId";
            return (await _dbConnection.QueryAsync<TeamMap>(query, new { teamId })).First();

        }
        public async Task<int> GetCurrentWeek(int season)
        {
            var query = $@"SELECT COALESCE(Max(Week), 0) + 1 
                        FROM [dbo].WeeklyQBData
                        WHERE [Season] = @season";
            return (await _dbConnection.QueryAsync<int>(query, new { season })).FirstOrDefault();
        }
        public async Task<List<Schedule>> GetUpcomingGames(int teamId, int season, int currentWeek)
        {
            var query = $@"SELECT [Season], [TeamId], [Team], [Week], [OpposingTeamId], [OpposingTeam]
                        FROM [dbo].Schedule
                        WHERE [TeamId] = @teamId
                            AND [Week] >= @currentWeek
                            AND [Season] = @season";
            return (await _dbConnection.QueryAsync<Schedule>(query, new { teamId, season, currentWeek })).ToList();
        }
        public async Task<List<Schedule>> GetGames(int season, int week)
        {
            var query = $@"SELECT [Season], [TeamId], [Team], [Week], [OpposingTeamId], [OpposingTeam]
                        FROM [dbo].Schedule
                        WHERE [Season] = @season
                        AND [Week] = @week";
            return (await _dbConnection.QueryAsync<Schedule>(query, new {season, week})).ToList();
        }
        public async Task<List<Schedule>> GetTeamGames(int teamId, int season)
        {
            var query = $@"SELECT [Season], [TeamId], [Team], [Week], [OpposingTeamId], [OpposingTeam]
                        FROM [dbo].Schedule
                        WHERE [TeamId] = @teamId
                            AND [Season] = @season";
            return (await _dbConnection.QueryAsync<Schedule>(query, new { teamId, season })).ToList();
        }
        public async Task<List<int>> GetIgnoreList()
        {
            var query = $@"SELECT [PlayerId] FROM [dbo].IgnoreList";
            return (await _dbConnection.QueryAsync<int>(query)).ToList();
        }
        public async Task<TeamLocation> GetTeamLocation(int teamId)
        {
            var query = $@"SELECT * FROM [dbo].TeamLocation
                            WHERE [TeamId] = @teamId";
            return (await _dbConnection.QueryAsync<TeamLocation>(query, new { teamId })).First();
        }
        public async Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].ScheduleDetails
                            WHERE [Season] = @season
                                AND [Week] = @week";
            return (await _dbConnection.QueryAsync<ScheduleDetails>(query, new {season, week})).ToList();
        }
        public async Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season)
        {
            var query = $@"SELECT * FROM [dbo].InSeasonInjuries
                            WHERE [Season] = @season
                                AND [InjuryEndWeek] = 0";
            return (await _dbConnection.QueryAsync<InSeasonInjury>(query, new { season })).ToList();                                          
        }

        public async Task<int> PostInSeasonInjury(InSeasonInjury injury)
        {
            var query = $@"INSERT INTO [dbo].InSeasonInjuries (Season, PlayerId, InjuryStartWeek, InjuryEndWeek, Description)
                            VALUES (@Season, @PlayerId, @InjuryStartWeek, @InjuryEndWeek, @Description)";
            return await _dbConnection.ExecuteAsync(query, injury);
        }

        public async Task<bool> UpdateInjury(InSeasonInjury injury)
        {
            var injuryId = injury.InjuryId;
            var injuryStartWeek = injury.InjuryStartWeek;
            var injuryEndWeek = injury.InjuryEndWeek;
            var desc = injury.Description;

            var query = $@"UPDATE [dbo].InSeasonInjuries
                            SET [InjuryStartWeek] = @injuryStartWeek,
                                [InjuryEndWeek] = @injuryEndWeek,
                                [Description] = @desc
                            WHERE [InjuryId] = @injuryId";
            return await _dbConnection.ExecuteAsync(query, new { injuryId, injuryStartWeek, injuryEndWeek, desc }) > 0;
        }

        public async Task<List<InSeasonTeamChange>> GetInSeasonTeamChanges(int season)
        {
            var query = $@"SELECT * FROM [dbo].InSeasonTeamChanges
                           WHERE [Season] = @season";
            return (await _dbConnection.QueryAsync<InSeasonTeamChange>(query, new { season })).ToList();
        }

        public async Task<bool> UpdateCurrentTeam(int playerId, string team, int season)
        {
            var query = $@"UPDATE [dbo].PlayerTeam
                           SET [Team] = @team
                           WHERE [PlayerId] = @playerId
                                AND [Season] = @season";
            return await _dbConnection.ExecuteAsync(query, new { playerId, team, season }) > 0;
        }

        public async Task<int> PostTeamChange(InSeasonTeamChange teamChange)
        {
            var query = $@"INSERT INTO [dbo].InSeasonTeamChanges (Season, PlayerId, PreviousTeam, NewTeam, WeekEffective)
                           VALUES (@Season, @PlayerId, @PreviousTeam, @NewTeam, @WeekEffective)";
            return await _dbConnection.ExecuteAsync(query, teamChange);
        }

        public async Task<int> InactivatePlayers(List<int> playerIds)
        {
            var query = $@"UPDATE [dbo].Players
                            Set [Active] = 0
                            WHERE [PlayerId] IN @playerIds";
            return await _dbConnection.ExecuteAsync(query, new { playerIds });
        }

        public async Task<List<int>> GetSeasons()
        {
            var query = $@"SELECT DISTINCT [Season]
                            FROM [dbo].Schedule
                            ORDER BY [Season] DESC";
            return (await _dbConnection.QueryAsync<int>(query)).ToList();
        }

        public async Task<bool> CreateRookie(Rookie rookie)
        {
            var query = $@"INSERT INTO [dbo].Rookie (PlayerId, TeamDrafted, Position, RookieSeason, DraftPosition, DeclareAge)
                            VALUES (@PlayerId, @TeamDrafted, @Position, @RookieSeason, @DraftPosition, @DeclareAge)";
            return await _dbConnection.ExecuteAsync(query, rookie) > 0;
        }
    }
}
