using Football.Interfaces;

namespace Football.Services
{
    public class SqlQueryService : ISqlQueryService
    {
        public string GetQueryString(string type)
        {
            switch (type) {

                case "passing":
                return $@"INSERT INTO [dbo].Passing (Season, Name, Team, Age, Games, Completions, Attempts, Yards, Touchdowns, Interceptions, FirstDowns, Long, Sacks, SackYards) 
                        VALUES(@season, @name, @team, @age, @games, @completions, @attempts, @yards, @touchdowns, @interceptions, @firstdowns, @long, @sacks, @sackyards)";
                              
                case "receiving":
                    return $@"INSERT INTO [dbo].Receiving (Season, Name, Team, Age, Games, Targets, Receptions, Yards, Touchdowns, FirstDowns, Long, RpG, Fumbles)
                            VALUES(@season, @name, @team, @age, @games, @targets, @receptions, @yards, @touchdowns, @firstdowns, @long, @rpg, @fumbles)";

                case "rushing":
                    return $@"INSERT INTO [dbo].Rushing (Season, Name, Team, Age, Games, RushAttempts, Yards, Touchdowns, FirstDowns, Long, Fumbles)
                            VALUES(@season, @name, @team, @age, @games, @rushattempts, @yards, @touchdowns, @firstdowns, @long, @fumbles)";

                default: return "";
            }
        }


        public string FantasyPassingQuery()
        {
            return $@"SELECT [PlayerId], [Season], [Yards], [Touchdowns], [Interceptions] 
                        FROM [dbo].Passing
                        WHERE [PlayerId] = @playerid
                        AND [Season] = @season";
        }

        public string FantasyRushingQuery()
        {
            return $@"SELECT [PlayerId], [Season], [Yards], [Touchdowns], [Fumbles] 
                        FROM [dbo].Rushing
                        WHERE [PlayerId] = @playerid
                        AND [Season] = @season";
        }

        public string FantasyReceivingQuery()
        {
            return $@"SELECT [PlayerId], [Season], [Yards], [Touchdowns], [Receptions] 
                        FROM [dbo].Receiving
                        WHERE [PlayerId] = @playerid
                        AND [Season] = @season";
        }

        public string GetPlayerIds()
        {
            return $@"SELECT [PlayerId] FROM [dbo].Players";
        }

        public string InsertFantasyData()
        {
            return $@"INSERT INTO [dbo].FantasyPoints (Season, PlayerId, TotalPoints, PassingPoints, RushingPoints, ReceivingPoints)
                    VALUES(@season, @playerid, @totalpoints, @passingpoints, @rushingpoints, @receivingpoints)";
        }

        public string GetPlayerPosition()
        {
            return $@"SELECT [Position] from [dbo].Players WHERE [PlayerId] = @playerid";
        }

        public string GetPlayersByPosition()
        {
            return $@"SELECT [PlayerId] from [dbo].Players WHERE [Position] = @position";
        }

        public string GetPassingStatistic()
        {
            return $@"SELECT [Name], [Team], [Age], [Games], [Completions], [Attempts],
                    [Yards], [Touchdowns], [Interceptions], [FirstDowns], [Long], [Sacks], [SackYards]
             FROM [dbo].Passing WHERE [Season] = @season AND [PlayerId] = @playerid";
        }

        public string GetPassingStatisticWithSeason()
        {
            return $@"SELECT [Season], [Name], [Team], [Age], [Games], [Completions], [Attempts],
                    [Yards], [Touchdowns], [Interceptions], [FirstDowns], [Long], [Sacks], [SackYards]
             FROM [dbo].Passing WHERE [Season] = @season AND [PlayerId] = @playerid";
        }

        public string GetRushingStatistic()
        {
            return $@"SELECT [Name], [Team], [Age], [Games], [RushAttempts], [Yards], [Touchdowns], [FirstDowns], [Long], [Fumbles]
                FROM [dbo].Rushing WHERE [Season] = @season AND [PlayerId] = @playerId";
        }
        public string GetRushingStatisticWithSeason()
        {
            return $@"SELECT [Season], [Name], [Team], [Age], [Games], [RushAttempts], [Yards], [Touchdowns], [FirstDowns], [Long], [Fumbles]
                FROM [dbo].Rushing WHERE [Season] = @season AND [PlayerId] = @playerId";
        }

        public string GetReceivingStatistic()
        { 
            return $@"SELECT [Name], [Team], [Age], [Games], [Targets], [Receptions], [Yards], [Touchdowns], [FirstDowns], [Long], [RpG], [Fumbles]
                FROM [dbo].Receiving WHERE [Season] = @season AND [PlayerId] = @playerId";
        }

        public string GetReceivingStatisticWithSeason()
        {
            return $@"SELECT [Season], [Name], [Team], [Age], [Games], [Targets], [Receptions], [Yards], [Touchdowns], [FirstDowns], [Long], [RpG], [Fumbles]
                FROM [dbo].Receiving WHERE [Season] = @season AND [PlayerId] = @playerId";
        }

        public string GetFantasyPoints()
        {
            return $@"SELECT [Season], [PlayerId], [TotalPoints] FROM [dbo].FantasyPoints WHERE [PlayerId] = @playerId AND [Season] = @season";
        }

        public string GetPlayerIdsByFantasySeason()
        {
            return $@"SELECT [PlayerId] from [dbo].FantasyPoints WHERE [Season] = @season";
        }

        public string DeleteFantasyPoints()
        {
            return $@"DELETE FROM dbo.FantasyPoints WHERE [PlayerId] = @playerId AND [Season] = @season";
        }

        public string GetSeasons()
        {
            return $@"SELECT [Season] FROM dbo.[FantasyPoints] WHERE [PlayerId] = @playerId GROUP BY [Season]";
        }

        public string GetActivePassingSeasons()
        {
            return $@"SELECT [Season] FROM dbo.[Passing] WHERE [PlayerId] = @playerId GROUP BY [Season]";
        }

        public string GetActiveRushingSeasons()
        {
            return $@"SELECT [Season] FROM dbo.[Rushing] WHERE [PlayerId] = @playerId GROUP BY [Season]";
        }

        public string GetActiveReceivingSeasons()
        {
            return $@"SELECT [Season] FROM dbo.[Receiving] WHERE [PlayerId] = @playerId GROUP BY [Season]";
        }
        public string GetQbGames()
        {
            return $@"SELECT [Season], [Games] FROM [dbo].Passing WHERE [PlayerId] = @playerId";
        }

        public string GetRbGames()
        {
            return $@"SELECT [Season], [Games] FROM [dbo].Rushing WHERE [PlayerId] = @playerId";
        }

        public string GetPcGames()
        {
            return $@"SELECT [Season], [Games] FROM [dbo].Receiving WHERE [PlayerId] = @playerId";
        }

        public string GetPlayerName()
        {
            return $@"SELECT [Name] FROM [dbo].Players WHERE [PlayerId] = @playerId";
        }

        public string IsPlayerActive()
        {
            return $@"SELECT [Active] FROM [dbo].Players WHERE [PlayerId] = @playerId";
        }

        public string GetPlayerTeam()
        {
            return $@"WITH allSeasons AS(
                        SELECT [PlayerId], [Team], [Season] FROM [dbo].Passing
                        UNION (SELECT [PlayerId], [Team], [Season] FROM [dbo].Receiving)
                        UNION (SELECT [PlayerId], [Team], [Season] FROM [dbo].Rushing))
                        SELECT [Team] FROM allSeasons a
                        WHERE [Season] = (Select MAX(a1.Season) from allSeasons a1 where a.PlayerId = a1.playerId)
                        and a.PlayerId = @playerId";
        }
        public string GetTightEnds()
        {
            return $@"SELECT [PlayerId] from [dbo].TightEnds";
        }
        public string InsertFantasyProjections()
        {
            return $@"INSERT INTO [dbo].FantasyProjections (Season, PlayerId, Name, Position, Rank, ProjectedPoints) VALUES
                    (@season, @playerid, @name, @position, @rank, @projectedpoints)";
        }

        public string GetPlayerInfo()
        {
            return $@"SELECT [PlayerId], [Name], [Position], [Active]
                    FROM [dbo].Players
                    WHERE [Playerid] = @playerId";
        }

        public string GetPassingStatisticsWithSeason()
        {
            return $@"SELECT [Season], [Name], [Team], [Age], [Games], [Completions], [Attempts],
                    [Yards], [Touchdowns], [Interceptions], [FirstDowns], [Long], [Sacks], [SackYards], [PlayerId]
             FROM [dbo].Passing WHERE  [PlayerId] = @playerid";
        }
        public string GetRushingStatisticsWithSeason()
        {
            return $@"SELECT [Season], [Name], [Team], [Age], [Games], [RushAttempts], [Yards], [Touchdowns], [FirstDowns], [Long], [Fumbles], [PlayerId]
                FROM [dbo].Rushing WHERE [PlayerId] = @playerId";
        }
        public string GetReceivingStatisticsWithSeason()
        {
            return $@"SELECT [Season], [Name], [Team], [Age], [Games], [Targets], [Receptions], [Yards], [Touchdowns], [FirstDowns], [Long], [RpG], [Fumbles], [PlayerId]
                FROM [dbo].Receiving WHERE [PlayerId] = @playerId";
        }
        public string GetAllFantasyResults()
        {
            return $@"SELECT [Season], [PlayerId], [TotalPoints], [PassingPoints], [RushingPoints], [ReceivingPoints]
                    FROM [dbo].FantasyPoints WHERE [PlayerId] = @playerId";
        }

        public string GetPlayerId()
        {
            return $@"SELECT [PlayerId] FROM [dbo].Players WHERE [Name] LIKE '%' + @name + '%'";
        }
    }
}
