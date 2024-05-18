using System.Data;
using System.Runtime.CompilerServices;
using Dapper;
using Football.Players.Interfaces;
using Football.Players.Models;

namespace Football.Players.Repository
{
    
    public class PlayersRepository(IDbConnection dbConnection) : IPlayersRepository
    {
        public async Task<int> GetPlayerId(string name)
        {
            var query = $@"SELECT [PlayerId] FROM [dbo].Players
                            WHERE [Name] = @name
                            ORDER BY [Active] DESC";
            return (await dbConnection.QueryAsync<int>(query, new { name })).FirstOrDefault();
        }
        public async Task<int> CreatePlayer(string name, int active, string position)
        {
            var query = $@"INSERT INTO [dbo].Players (Name, Position, Active)
                        OUTPUT INSERTED.PlayerId
                        VALUES (@name, @position, @active)";
            return await dbConnection.QuerySingleAsync<int>(query, new {name, active, position});
        }
        public async Task<List<Player>> GetAllPlayers()
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Active]
                        FROM [dbo].Players  
                        ";
            return (await dbConnection.QueryAsync<Player>(query)).ToList();
        }
        public async Task<List<Player>> GetPlayersByPosition(string position)
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Active]
                            FROM [dbo].Players
                            WHERE [Position] = @position";
            return (await dbConnection.QueryAsync<Player>(query, new { position })).ToList();
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Active]
                        FROM [dbo].Players
                        WHERE [PlayerId] = @playerId";
            return (await dbConnection.QueryAsync<Player>(query, new { playerId })).First();
        }

        public async Task<Player?> GetPlayerByName(string name)
        {
            var query = $@"SELECT * FROM [dbo].Players
                            WHERE [Name] = @name
                            ORDER BY [Active] DESC";
            return (await dbConnection.QueryAsync<Player>(query, new { name })).FirstOrDefault();
        }
        public async Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position)
        {
            var query = $@"SELECT [PlayerId], [TeamDrafted], [Position], [RookieSeason], [DraftPosition], [DeclareAge]
                        FROM [dbo].Rookie
                        WHERE [RookieSeason] < @currentSeason
                        AND [Position] = @position";
            return (await dbConnection.QueryAsync<Rookie>(query, new {currentSeason, position})).ToList();
        }
        public async Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position)
        {
            var query = $@"SELECT [PlayerId], [TeamDrafted], [Position], [RookieSeason], [DraftPosition], [DeclareAge]
                        FROM [dbo].Rookie
                        WHERE [RookieSeason] = @currentSeason
                        AND [Position] = @position";
            return (await dbConnection.QueryAsync<Rookie>(query, new { currentSeason, position })).ToList();
        }
        public async Task<List<InjuryConcerns>> GetPlayerInjuries(int season)
        {
            var query = $@"SELECT * FROM [dbo].InjuryConcerns
                        WHERE [Season] = @season";
            return (await dbConnection.QueryAsync<InjuryConcerns>(query, new { season })).ToList();
        }
        public async Task<List<Suspensions>> GetPlayerSuspensions(int season)
        {
            var query = $@"SELECT * FROM [dbo].Suspensions
                            WHERE [Season] = @season";                            
            return (await dbConnection.QueryAsync<Suspensions>(query, new { season })).ToList();
        }
        public async Task<List<QuarterbackChange>> GetQuarterbackChanges(int season)
        {
            var query = $@"SELECT [PlayerId], [Season], [PreviousQB], [CurrentQB]
                        FROM [dbo].QuarterbackChanges
                        WHERE [Season] = @season";
            return (await dbConnection.QueryAsync<QuarterbackChange>(query, new {season})).ToList();
        }
        public async Task<double> GetEPA(int playerId, int season)
        {
            var query = $@"SELECT [EPA] FROM [dbo].[EPA] WHERE [PlayerId] = @playerId AND [Season] = @season";
            return (await dbConnection.QueryAsync<double>(query, new {playerId, season})).FirstOrDefault();
        }
        public async Task<double> GetSeasonProjection(int season, int playerId)
        {
            var query = $@"SELECT [ProjectedPoints]
                        FROM [dbo].SeasonProjections
                        WHERE [PlayerId] = @playerId
                            AND [Season] = @season";
            return (await dbConnection.QueryAsync<double>(query, new { season, playerId })).FirstOrDefault(); 
        }

        public async Task<double> GetWeeklyProjection(int season, int week, int playerId)
        {
            var query = $@"SELECT [ProjectedPoints]
                            FROM [dbo].WeeklyProjections
                            WHERE [Season] = @season
                                AND [Week] = @week
                                AND [PlayerId] = @playerId";
            return (await dbConnection.QueryAsync<double>(query, new { season, week, playerId })).FirstOrDefault();
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

            return (await dbConnection.QueryAsync<PlayerTeam>(query, new { team , season})).ToList();
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
            return (await dbConnection.QueryAsync<int>(query, new {teamDescription})).FirstOrDefault();
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
        public async Task<int> GetCurrentWeek(int season)
        {
            var query = $@"SELECT COALESCE(Max(Week), 0) + 1 
                        FROM [dbo].WeeklyQBData
                        WHERE [Season] = @season";
            return (await dbConnection.QueryAsync<int>(query, new { season })).FirstOrDefault();
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
        public async Task<List<Schedule>> GetGames(int season, int week)
        {
            var query = $@"SELECT [Season], [TeamId], [Team], [Week], [OpposingTeamId], [OpposingTeam]
                        FROM [dbo].Schedule
                        WHERE [Season] = @season
                        AND [Week] = @week";
            return (await dbConnection.QueryAsync<Schedule>(query, new {season, week})).ToList();
        }
        public async Task<List<Schedule>> GetTeamGames(int teamId, int season)
        {
            var query = $@"SELECT [Season], [TeamId], [Team], [Week], [OpposingTeamId], [OpposingTeam]
                        FROM [dbo].Schedule
                        WHERE [TeamId] = @teamId
                            AND [Season] = @season";
            return (await dbConnection.QueryAsync<Schedule>(query, new { teamId, season })).ToList();
        }
        public async Task<List<int>> GetIgnoreList()
        {
            var query = $@"SELECT [PlayerId] FROM [dbo].IgnoreList";
            return (await dbConnection.QueryAsync<int>(query)).ToList();
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
            return (await dbConnection.QueryAsync<ScheduleDetails>(query, new {season, week})).ToList();
        }
        public async Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season)
        {
            var query = $@"SELECT * FROM [dbo].InSeasonInjuries
                            WHERE [Season] = @season
                                AND [InjuryEndWeek] = 0";
            return (await dbConnection.QueryAsync<InSeasonInjury>(query, new { season })).ToList();                                          
        }

        public async Task<int> PostInSeasonInjury(InSeasonInjury injury)
        {
            var query = $@"INSERT INTO [dbo].InSeasonInjuries (Season, PlayerId, InjuryStartWeek, InjuryEndWeek, Description)
                            VALUES (@Season, @PlayerId, @InjuryStartWeek, @InjuryEndWeek, @Description)";
            return await dbConnection.ExecuteAsync(query, injury);
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
            return await dbConnection.ExecuteAsync(query, new { injuryId, injuryStartWeek, injuryEndWeek, desc }) > 0;
        }

        public async Task<List<InSeasonTeamChange>> GetInSeasonTeamChanges(int season)
        {
            var query = $@"SELECT * FROM [dbo].InSeasonTeamChanges
                           WHERE [Season] = @season";
            return (await dbConnection.QueryAsync<InSeasonTeamChange>(query, new { season })).ToList();
        }

        public async Task<bool> UpdateCurrentTeam(int playerId, string team, int season)
        {
            var query = $@"UPDATE [dbo].PlayerTeam
                           SET [Team] = @team
                           WHERE [PlayerId] = @playerId
                                AND [Season] = @season";
            return await dbConnection.ExecuteAsync(query, new { playerId, team, season }) > 0;
        }

        public async Task<int> PostTeamChange(InSeasonTeamChange teamChange)
        {
            var query = $@"INSERT INTO [dbo].InSeasonTeamChanges (Season, PlayerId, PreviousTeam, NewTeam, WeekEffective)
                           VALUES (@Season, @PlayerId, @PreviousTeam, @NewTeam, @WeekEffective)";
            return await dbConnection.ExecuteAsync(query, teamChange);
        }

        public async Task<int> InactivatePlayers(List<int> playerIds)
        {
            var query = $@"UPDATE [dbo].Players
                            Set [Active] = 0
                            WHERE [PlayerId] IN @playerIds";
            return await dbConnection.ExecuteAsync(query, new { playerIds });
        }

        public async Task<List<int>> GetSeasons()
        {
            var query = $@"SELECT DISTINCT [Season]
                            FROM [dbo].Schedule
                            ORDER BY [Season] DESC";
            return (await dbConnection.QueryAsync<int>(query)).ToList();
        }

        public async Task<bool> CreateRookie(Rookie rookie)
        {
            var query = $@"INSERT INTO [dbo].Rookie (PlayerId, TeamDrafted, Position, RookieSeason, DraftPosition, DeclareAge)
                            VALUES (@PlayerId, @TeamDrafted, @Position, @RookieSeason, @DraftPosition, @DeclareAge)";
            return await dbConnection.ExecuteAsync(query, rookie) > 0;
        }

        public async Task<List<Rookie>> GetAllRookies()
        {
            var query = $@"SELECT * FROM [dbo].Rookie";
            return (await dbConnection.QueryAsync<Rookie>(query)).ToList();
        }

        public async Task<IEnumerable<Schedule>> GetWeeklySchedule(int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].Schedule
                            WHERE [Season] = @season
                                AND [Week] = @week";
            return await dbConnection.QueryAsync<Schedule>(query, new { season, week });
        }

        public async Task<int> UploadSleeperPlayerMap(List<SleeperPlayerMap> playerMap)
        {
            var query = $@"INSERT INTO [dbo].SleeperPlayerMap (SleeperPlayerId, PlayerId) VALUES (@SleeperPlayerId, @PlayerId)";
            return await dbConnection.ExecuteAsync(query, playerMap);
        }
        public async Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId)
        {
            var query = $@"SELECT * FROM [dbo].SleeperPlayerMap WHERE SleeperPlayerId = @sleeperId";
            return (await dbConnection.QueryAsync<SleeperPlayerMap>(query, new { sleeperId })).FirstOrDefault();
        }

        public async Task<List<SleeperPlayerMap>> GetSleeperPlayerMaps()
        {
            var query = $@"SELECT * FROM [dbo].SleeperPlayerMap";
            return (await dbConnection.QueryAsync<SleeperPlayerMap>(query)).ToList();
        }
    }
}
