USE [master]
GO
CREATE DATABASE [Football]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Football', FILENAME = N'C:-----\Football.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Football_log', FILENAME = N'C:----\Football_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Football] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Football].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Football] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Football] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Football] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Football] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Football] SET ARITHABORT OFF 
GO
ALTER DATABASE [Football] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Football] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Football] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Football] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Football] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Football] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Football] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Football] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Football] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Football] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Football] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Football] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Football] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Football] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Football] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Football] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Football] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Football] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Football] SET  MULTI_USER 
GO
ALTER DATABASE [Football] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Football] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Football] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Football] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Football] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Football] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Football] SET QUERY_STORE = OFF
GO
USE [Football]
GO
/****** Object:  Table [dbo].[ADP]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ADP](
	[Season] [int] NULL,
	[PlayerId] [int] NULL,
	[Name] [varchar](50) NULL,
	[Position] [varchar](5) NULL,
	[PositionADP] [float] NULL,
	[OverallADP] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EPA]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EPA](
	[PlayerId] [int] NULL,
	[Season] [int] NULL,
	[EPA] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GameResults]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameResults](
	[Season] [int] NULL,
	[WinnerId] [int] NULL,
	[LoserId] [int] NULL,
	[HomeTeamId] [int] NULL,
	[AwayTeamId] [int] NULL,
	[Week] [int] NULL,
	[Day] [varchar](5) NULL,
	[Date] [varchar](15) NULL,
	[Time] [varchar](15) NULL,
	[Winner] [varchar](50) NULL,
	[Loser] [varchar](50) NULL,
	[WinnerPoints] [int] NULL,
	[LoserPoints] [int] NULL,
	[WinnerYards] [int] NULL,
	[LoserYards] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IgnoreList]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IgnoreList](
	[PlayerId] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InjuryConcerns]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InjuryConcerns](
	[PlayerId] [int] NULL,
	[Season] [int] NULL,
	[Games] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InSeasonInjuries]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InSeasonInjuries](
	[Season] [int] NULL,
	[PlayerId] [int] NULL,
	[InjuryStartWeek] [int] NULL,
	[InjuryEndWeek] [int] NULL,
	[Description] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InSeasonTeamChanges]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InSeasonTeamChanges](
	[Season] [int] NULL,
	[PlayerId] [int] NULL,
	[PreviousTeam] [varchar](3) NULL,
	[NewTeam] [varchar](3) NULL,
	[WeekEffective] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MockDraftFantasyTeams]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MockDraftFantasyTeams](
	[FantasyTeamId] [int] IDENTITY(1,1) NOT NULL,
	[MockDraftID] [int] NULL,
	[DraftPosition] [int] NULL,
	[FantasyTeamName] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MockDraftLookup]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MockDraftLookup](
	[MockDraftID] [int] IDENTITY(1,1) NOT NULL,
	[TeamCount] [int] NULL,
	[UserPosition] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MockDraftResults]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MockDraftResults](
	[MockDraftID] [int] NULL,
	[Round] [int] NULL,
	[FantasyTeamId] [int] NULL,
	[Pick] [int] NULL,
	[PlayerId] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Players]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Players](
	[PlayerId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[Position] [varchar](5) NULL,
	[Active] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlayerTeam]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayerTeam](
	[PlayerId] [int] NULL,
	[Name] [varchar](50) NULL,
	[Season] [int] NULL,
	[Team] [varchar](5) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuarterbackChanges]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuarterbackChanges](
	[PlayerId] [int] NULL,
	[Season] [int] NULL,
	[PreviousQB] [int] NULL,
	[CurrentQB] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rookie]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rookie](
	[PlayerId] [int] NULL,
	[TeamDrafted] [varchar](3) NULL,
	[Position] [varchar](5) NULL,
	[RookieSeason] [int] NULL,
	[DraftPosition] [int] NULL,
	[DeclareAge] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Schedule]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schedule](
	[Season] [int] NULL,
	[TeamId] [int] NULL,
	[Team] [varchar](5) NULL,
	[Week] [int] NULL,
	[OpposingTeamId] [int] NULL,
	[OpposingTeam] [varchar](5) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduleDetails]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleDetails](
	[Season] [int] NULL,
	[Week] [int] NULL,
	[Day] [varchar](3) NULL,
	[Date] [varchar](11) NULL,
	[Time] [varchar](10) NULL,
	[HomeTeamId] [int] NULL,
	[AwayTeamId] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SeasonDSTData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SeasonDSTData](
	[Season] [int] NULL,
	[PlayerID] [int] NULL,
	[Name] [varchar](50) NULL,
	[Sacks] [float] NULL,
	[Ints] [float] NULL,
	[FumblesRecovered] [float] NULL,
	[ForcedFumbles] [float] NULL,
	[DefensiveTD] [float] NULL,
	[Safties] [float] NULL,
	[SpecialTD] [float] NULL,
	[Games] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SeasonFantasyData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SeasonFantasyData](
	[PlayerId] [int] NULL,
	[Season] [int] NULL,
	[Games] [float] NULL,
	[FantasyPoints] [float] NULL,
	[Name] [varchar](50) NULL,
	[Position] [varchar](3) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SeasonProjections]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SeasonProjections](
	[PlayerId] [int] NULL,
	[Season] [int] NULL,
	[Name] [varchar](50) NULL,
	[Position] [varchar](5) NULL,
	[ProjectedPoints] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SeasonQBData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SeasonQBData](
	[Season] [int] NULL,
	[PlayerID] [int] NULL,
	[Name] [varchar](50) NULL,
	[Completions] [float] NULL,
	[Attempts] [float] NULL,
	[Yards] [float] NULL,
	[TD] [float] NULL,
	[Int] [float] NULL,
	[Sacks] [float] NULL,
	[RushingAttempts] [float] NULL,
	[RushingYards] [float] NULL,
	[RushingTD] [float] NULL,
	[Fumbles] [float] NULL,
	[Games] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SeasonRBData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SeasonRBData](
	[Season] [int] NULL,
	[PlayerID] [int] NULL,
	[Name] [varchar](50) NULL,
	[RushingAtt] [float] NULL,
	[RushingYds] [float] NULL,
	[RushingTD] [float] NULL,
	[Receptions] [float] NULL,
	[Targets] [float] NULL,
	[Yards] [float] NULL,
	[ReceivingTD] [float] NULL,
	[Fumbles] [float] NULL,
	[Games] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SeasonTEData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SeasonTEData](
	[Season] [int] NULL,
	[PlayerID] [int] NULL,
	[Name] [varchar](50) NULL,
	[Receptions] [float] NULL,
	[Targets] [float] NULL,
	[Yards] [float] NULL,
	[Long] [float] NULL,
	[TD] [float] NULL,
	[RushingAtt] [float] NULL,
	[RushingYds] [float] NULL,
	[RushingTD] [float] NULL,
	[Fumbles] [float] NULL,
	[Games] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SeasonWRData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SeasonWRData](
	[Season] [int] NULL,
	[PlayerID] [int] NULL,
	[Name] [varchar](50) NULL,
	[Receptions] [float] NULL,
	[Targets] [float] NULL,
	[Yards] [float] NULL,
	[Long] [float] NULL,
	[TD] [float] NULL,
	[RushingAtt] [float] NULL,
	[RushingYds] [float] NULL,
	[RushingTD] [float] NULL,
	[Fumbles] [float] NULL,
	[Games] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SleeperPlayerMap]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SleeperPlayerMap](
	[SleeperPlayerId] [int] NULL,
	[PlayerId] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Suspensions]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suspensions](
	[PlayerId] [int] NULL,
	[Season] [int] NULL,
	[Length] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeamChanges]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeamChanges](
	[PlayerId] [int] NULL,
	[Season] [int] NULL,
	[PreviousTeam] [varchar](3) NULL,
	[NewTeam] [varchar](3) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeamLocation]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeamLocation](
	[TeamId] [int] NULL,
	[City] [varchar](50) NULL,
	[State] [varchar](2) NULL,
	[Zip] [varchar](5) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeamMap]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeamMap](
	[TeamId] [int] IDENTITY(1,1) NOT NULL,
	[Team] [varchar](5) NULL,
	[TeamDescription] [varchar](50) NULL,
	[PlayerId] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WeeklyDSTData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WeeklyDSTData](
	[Season] [int] NULL,
	[Week] [int] NULL,
	[PlayerId] [int] NULL,
	[Name] [varchar](50) NULL,
	[Sacks] [float] NULL,
	[Ints] [float] NULL,
	[FumblesRecovered] [float] NULL,
	[ForcedFumbles] [float] NULL,
	[DefensiveTD] [float] NULL,
	[Safties] [float] NULL,
	[SpecialTD] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WeeklyFantasyData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WeeklyFantasyData](
	[PlayerId] [int] NULL,
	[Name] [varchar](50) NULL,
	[Position] [varchar](5) NULL,
	[Season] [int] NULL,
	[Week] [int] NULL,
	[Games] [int] NULL,
	[FantasyPoints] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WeeklyProjections]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WeeklyProjections](
	[PlayerId] [int] NULL,
	[Season] [int] NULL,
	[Week] [int] NULL,
	[Name] [varchar](50) NULL,
	[Position] [varchar](5) NULL,
	[ProjectedPoints] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WeeklyQBData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WeeklyQBData](
	[Season] [int] NULL,
	[Week] [int] NULL,
	[PlayerId] [int] NULL,
	[Name] [varchar](50) NULL,
	[Completions] [float] NULL,
	[Attempts] [float] NULL,
	[Yards] [float] NULL,
	[TD] [float] NULL,
	[Int] [float] NULL,
	[Sacks] [float] NULL,
	[RushingAttempts] [float] NULL,
	[RushingYards] [float] NULL,
	[RushingTD] [float] NULL,
	[Fumbles] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WeeklyRBData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WeeklyRBData](
	[Season] [int] NULL,
	[Week] [int] NULL,
	[PlayerId] [int] NULL,
	[Name] [varchar](50) NULL,
	[RushingAtt] [float] NULL,
	[RushingYds] [float] NULL,
	[RushingTD] [float] NULL,
	[Receptions] [float] NULL,
	[Targets] [float] NULL,
	[Yards] [float] NULL,
	[ReceivingTD] [float] NULL,
	[Fumbles] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WeeklyRosterPercentages]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WeeklyRosterPercentages](
	[Season] [int] NULL,
	[Week] [int] NULL,
	[PlayerId] [int] NULL,
	[Name] [varchar](50) NULL,
	[RosterPercent] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WeeklyTEData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WeeklyTEData](
	[Season] [int] NULL,
	[Week] [int] NULL,
	[PlayerId] [int] NULL,
	[Name] [varchar](50) NULL,
	[Receptions] [float] NULL,
	[Targets] [float] NULL,
	[Yards] [float] NULL,
	[Long] [float] NULL,
	[TD] [float] NULL,
	[RushingAtt] [float] NULL,
	[RushingYds] [float] NULL,
	[RushingTD] [float] NULL,
	[Fumbles] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WeeklyWRData]    Script Date: 11/1/2023 1:58:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WeeklyWRData](
	[Season] [int] NULL,
	[Week] [int] NULL,
	[PlayerId] [int] NULL,
	[Name] [varchar](50) NULL,
	[Receptions] [float] NULL,
	[Targets] [float] NULL,
	[Yards] [float] NULL,
	[Long] [float] NULL,
	[TD] [float] NULL,
	[RushingAtt] [float] NULL,
	[RushingYds] [float] NULL,
	[RushingTD] [float] NULL,
	[Fumbles] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Index [season_week_teams]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [season_week_teams] ON [dbo].[GameResults]
(
	[Season] ASC,
	[Week] ASC,
	[WinnerId] ASC,
	[LoserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [playerid]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [playerid] ON [dbo].[IgnoreList]
(
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_id]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_id] ON [dbo].[Players]
(
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [season_team_week]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [season_team_week] ON [dbo].[Schedule]
(
	[Season] ASC,
	[TeamId] ASC,
	[Week] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season] ON [dbo].[SeasonFantasyData]
(
	[Season] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season] ON [dbo].[SeasonProjections]
(
	[Season] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season] ON [dbo].[SeasonQBData]
(
	[Season] ASC,
	[PlayerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season] ON [dbo].[SeasonRBData]
(
	[Season] ASC,
	[PlayerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season] ON [dbo].[SeasonTEData]
(
	[Season] ASC,
	[PlayerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season] ON [dbo].[SeasonWRData]
(
	[Season] ASC,
	[PlayerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season_week]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season_week] ON [dbo].[WeeklyFantasyData]
(
	[Season] ASC,
	[Week] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season_week]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season_week] ON [dbo].[WeeklyProjections]
(
	[Season] ASC,
	[Week] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season_week]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season_week] ON [dbo].[WeeklyQBData]
(
	[Season] ASC,
	[Week] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season_week]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season_week] ON [dbo].[WeeklyRBData]
(
	[Season] ASC,
	[Week] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [season_week_playerid]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [season_week_playerid] ON [dbo].[WeeklyRosterPercentages]
(
	[Season] ASC,
	[Week] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season_week]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season_week] ON [dbo].[WeeklyTEData]
(
	[Season] ASC,
	[Week] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [player_season_week]    Script Date: 11/1/2023 1:58:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [player_season_week] ON [dbo].[WeeklyWRData]
(
	[Season] ASC,
	[Week] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [Football] SET  READ_WRITE 
GO
