using Football.Players.Interfaces;
using Football.Players.Models;
using Dapper;
using System.Data;

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
        public async Task<List<Player>> GetAllPlayers(int active = 0, string position = "")
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Active]
                        FROM [dbo].Players  
                        WHERE 1=1";
            if (active > 0) query += " AND [Active] = @active";
            if (position != "") query += " AND [Position] = @position";
            return (await dbConnection.QueryAsync<Player>(query, new {active, position})).ToList();
        }
        public async Task<List<Player>> GetPlayersByPosition(string position, bool activeOnly)
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Active]
                            FROM [dbo].Players
                            WHERE [Position] = @position";

            if (activeOnly) query += $@" AND [Active] = 1";

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
            var query = $@"SELECT *
                        FROM [dbo].Rookie
                        WHERE [RookieSeason] < @currentSeason
                        AND [Position] = @position";
            return (await dbConnection.QueryAsync<Rookie>(query, new {currentSeason, position})).ToList();
        }
        public async Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position)
        {
            var query = $@"SELECT *
                        FROM [dbo].Rookie
                        WHERE [RookieSeason] = @currentSeason
                        AND [Position] = @position";
            return (await dbConnection.QueryAsync<Rookie>(query, new { currentSeason, position })).ToList();
        }
        public async Task<List<InjuryConcerns>> GetInjuryConcerns(int season)
        {
            var query = $@"SELECT * FROM [dbo].InjuryConcerns
                        WHERE [Season] = @season";
            return (await dbConnection.QueryAsync<InjuryConcerns>(query, new { season })).ToList();
        }
        public async Task<bool> DeleteInjuryConcern(int season, int playerId)
        {
            var query = $@"DELETE FROM [dbo].InjuryConcerns
                            WHERE [Season] = @Season
                                AND [PlayerId] = @PlayerId";
            return (await dbConnection.ExecuteAsync(query, new { season, playerId })) > 0;
        }

        public async Task<List<PlayerInjury>> GetPlayerInjuryHistory(int playerId)
        {
            var query = $@"SELECT * FROM [dbo].InSeasonInjuries
                            WHERE [PlayerId] = @playerId";
            return (await dbConnection.QueryAsync<PlayerInjury>(query, new { playerId })).ToList();
        }
        public async Task<List<Suspensions>> GetPlayerSuspensions(int season)
        {
            var query = $@"SELECT * FROM [dbo].Suspensions
                            WHERE [Season] = @season";                            
            return (await dbConnection.QueryAsync<Suspensions>(query, new { season })).ToList();
        }

        public async Task<Dictionary<int, double>> GetSeasonProjections(IEnumerable<int> playerIds, int season)
        {
            var query = $@"SELECT [PlayerId], [ProjectedPoints]
                        FROM [dbo].SeasonProjections
                        WHERE [PlayerId] IN @playerIds
                            AND [Season] = @season";
            return (await dbConnection.QueryAsync<(int PlayerId, double ProjectedPoints)>(query, new { playerIds, season })).ToDictionary(p => p.PlayerId, p => p.ProjectedPoints);
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
        public async Task<int> GetCurrentWeek(int season)
        {
            var query = $@"SELECT COALESCE(Max(Week), 0) + 1 
                        FROM [dbo].WeeklyQBData
                        WHERE [Season] = @season";
            return (await dbConnection.QueryAsync<int>(query, new { season })).FirstOrDefault();
        }

        public async Task<List<int>> GetIgnoreList()
        {
            var query = $@"SELECT [PlayerId] FROM [dbo].IgnoreList";
            return (await dbConnection.QueryAsync<int>(query)).ToList();
        }


        public async Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season)
        {
            var query = $@"SELECT * FROM [dbo].InSeasonInjuries
                            WHERE [Season] = @season
                                AND [InjuryEndWeek] = 0";
            return (await dbConnection.QueryAsync<InSeasonInjury>(query, new { season })).ToList();                                          
        }

        public async Task<Dictionary<int, double>> GetGamesPlayedInjuredBySeason(int season)
        {
            var query = $@"SELECT [PlayerId], SUM(GamesPlayedInjured)
                        FROM [dbo].InSeasonInjuries
                        WHERE [Season] = @season
                        GROUP BY [PlayerId]";                            
            return (await dbConnection.QueryAsync<(int PlayerId, double GamesPlayedInjured)>(query, new { season })).ToDictionary(p => p.PlayerId, p => p.GamesPlayedInjured);
        }

        public async Task<int> PostInSeasonInjury(InSeasonInjury injury)
        {
            var query = $@"INSERT INTO [dbo].InSeasonInjuries (Season, PlayerId, InjuryStartWeek, InjuryEndWeek, Description, GamesPlayedInjured)
                            VALUES (@Season, @PlayerId, @InjuryStartWeek, @InjuryEndWeek, @Description, @GamesPlayedInjured)";
            return await dbConnection.ExecuteAsync(query, injury);
        }

        public async Task<bool> UpdateInjury(InSeasonInjury injury)
        {
            var injuryId = injury.InjuryId;
            var injuryStartWeek = injury.InjuryStartWeek;
            var injuryEndWeek = injury.InjuryEndWeek;
            var desc = injury.Description;
            var injuredGames = injury.GamesPlayedInjured;

            var query = $@"UPDATE [dbo].InSeasonInjuries
                            SET [InjuryStartWeek] = @injuryStartWeek,
                                [InjuryEndWeek] = @injuryEndWeek,
                                [Description] = @desc,
                                [GamesPlayedInjured] = @injuredGames
                            WHERE [InjuryId] = @injuryId";
            return await dbConnection.ExecuteAsync(query, new { injuryId, injuryStartWeek, injuryEndWeek, desc, injuredGames }) > 0;
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

        public async Task<int> ActivatePlayer(int playerId)
        {
            var query = $@"UPDATE [dbo].Players SET [Active] = 1 WHERE [PlayerId] = @playerId";
            return await dbConnection.ExecuteAsync(query, new { playerId });
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
            var query = $@"INSERT INTO [dbo].Rookie (PlayerId, TeamDrafted, Position, RookieSeason, DraftPick, DeclareAge, TeamId)
                            VALUES (@PlayerId, @TeamDrafted, @Position, @RookieSeason, @DraftPick, @DeclareAge, @TeamId)";
            return await dbConnection.ExecuteAsync(query, rookie) > 0;
        }

        public async Task<List<Rookie>> GetAllRookies()
        {
            var query = $@"SELECT * FROM [dbo].Rookie";
            return (await dbConnection.QueryAsync<Rookie>(query)).ToList();
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

        public async Task<IEnumerable<TeamChange>> GetAllTeamChanges(int currentSeason, int previousSeason)
        {
            var query = $@"WITH previousSeasonTeams AS(
                                        SELECT * FROM PlayerTeam pt 
                                        WHERE pt.Season = @previousSeason),
                                currentSeasonTeams AS(
                                        SELECT * FROM PlayerTeam pt2
                                        where pt2.Season = @currentSeason)

                          SELECT p.PlayerId, p.Position, cst.Season, pst.TeamId AS PreviousTeamId, cst.TeamId AS CurrentTeamId 
                          FROM currentSeasonTeams cst 
                          JOIN previousSeasonTeams pst 
                          ON cst.PlayerId = pst.PlayerId
                          JOIN Players p
                          ON cst.PlayerId = p.PlayerId
                          WHERE cst.TeamId <> pst.TeamId
                            AND p.Active = 1";
            return await dbConnection.QueryAsync<TeamChange>(query, new { currentSeason, previousSeason });
        }

        public async Task<SeasonInfo?> GetSeasonInfo(int season)
        {
            var query = $@"SELECT * FROM SeasonInfo 
                            WHERE Season = @season";
            return (await dbConnection.QueryAsync<SeasonInfo>(query, new { season })).FirstOrDefault();
        }

        public async Task<bool> PostSeasonInfo(SeasonInfo season)
        {
            var query = $@"INSERT INTO dbo.[SeasonInfo] (Season, Games, Weeks) VALUES (@Season, @Games, @Weeks)";
            return (await dbConnection.ExecuteAsync(query, season) > 0);
        }

        public async Task<IEnumerable<BackupQuarterback>> GetBackupQuarterbacks(int season)
        {
            var query = $@"SELECT * FROM [dbo].BackupQuarterback WHERE [Season] = @season";
            return await dbConnection.QueryAsync<BackupQuarterback>(query, new { season });
        }

        public async Task<int> PostBackupQuarterbacks(List<BackupQuarterback> backups)
        {
            var query = $@"INSERT INTO [dbo].BackupQuarterback (Season, PlayerId) VALUES (@Season, @PlayerId)";
            return await dbConnection.ExecuteAsync(query, backups);
        }

        public async Task<int> DeleteAllBackupQuarterbacks(int season)
        {
            var query = $@"DELETE FROM [dbo].BackupQuarterback 
                            WHERE [Season] = @season";                            
            return await dbConnection.ExecuteAsync(query, new { season });
        }
            
    }
}
