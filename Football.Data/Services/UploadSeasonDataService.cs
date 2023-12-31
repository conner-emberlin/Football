﻿using Football.Models;
using Football.Enums;
using Football.Data.Interfaces;
using Football.Data.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;
using Serilog;

namespace Football.Data.Services
{
    public class UploadSeasonDataService(IScraperService scraperService, IUploadSeasonDataRepository uploadSeasonDataRepository,
         ILogger logger, IOptionsMonitor<WeeklyScraping> scraping, IPlayersService playerService, IOptionsMonitor<Season> season) : IUploadSeasonDataService
    {
        private readonly WeeklyScraping _scraping = scraping.CurrentValue;
        private readonly Season _season = season.CurrentValue;

        public async Task<int> UploadSeasonQBData(int season)
        {
            var url = FantasyProsURLFormatter(Position.QB.ToString(), season.ToString());
            var players = await SeasonDataQB(scraperService.ParseFantasyProsQBData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await uploadSeasonDataRepository.UploadSeasonQBData(players, await playerService.GetIgnoreList());
        }
        public async Task<int> UploadSeasonRBData(int season)
        {
            var url = FantasyProsURLFormatter(Position.RB.ToString(), season.ToString());
            var players = await SeasonDataRB(scraperService.ParseFantasyProsRBData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await uploadSeasonDataRepository.UploadSeasonRBData(players, await playerService.GetIgnoreList());
        }
        public async Task<int> UploadSeasonWRData(int season)
        {
            var url = FantasyProsURLFormatter(Position.WR.ToString(), season.ToString());
            var players = await SeasonDataWR(scraperService.ParseFantasyProsWRData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await uploadSeasonDataRepository.UploadSeasonWRData(players, await playerService.GetIgnoreList());
        }
        public async Task<int> UploadSeasonTEData(int season)
        {
            var url = FantasyProsURLFormatter(Position.TE.ToString(), season.ToString());
            var players = await SeasonDataTE(scraperService.ParseFantasyProsTEData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await uploadSeasonDataRepository.UploadSeasonTEData(players, await playerService.GetIgnoreList());
        }
        public async Task<int> UploadSeasonDSTData(int season)
        {
            var url = FantasyProsURLFormatter(Position.DST.ToString(), season.ToString());
            var players = await SeasonDataDST(scraperService.ParseFantasyProsDSTData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await uploadSeasonDataRepository.UploadSeasonDSTData(players);
        }
        public async Task<int> UploadCurrentTeams(int season, string position)
        {
            var url = FantasyProsURLFormatter(position, season.ToString());
            var str = scraperService.ScrapeData(url, _scraping.FantasyProsXPath);
            var parsedStr = scraperService.ParseFantasyProsPlayerTeam(str, position);
            foreach (var pt in parsedStr)
            {
                logger.Information("Getting team for player {playerId}", pt.PlayerId);
                pt.PlayerId = await playerService.GetPlayerId(pt.Name);
                if(pt.PlayerId == 0)
                {
                    logger.Information("Player {Name} does not exist in players table", pt.Name);
                }
                pt.Season = season;
            }
            return await uploadSeasonDataRepository.UploadCurrentTeams(parsedStr);
        }

        public async Task<int> UploadSchedule(int season)
        {
            var str = scraperService.ScrapeData(_scraping.UploadScheduleURL, _scraping.UploadScheduleXPath);
            var schedules = await scraperService.ParseFantasyProsSeasonSchedule(str);
            return await uploadSeasonDataRepository.UploadSchedule(schedules);
        }
        public async Task<int> UploadScheduleDetails(int season)
        {
            var count = 0;
            for (int i = 1; i <= _season.Games + 1; i++)
            {
                var details = await ScheduleDetails(await scraperService.ScrapeGameScores(i), season);
                count += await uploadSeasonDataRepository.UploadScheduleDetails(details);
            }
            return count;
        }

        public async Task<int> UploadADP(int season, string position) => await uploadSeasonDataRepository.UploadADP(await SeasonADP(await scraperService.ScrapeADP(position), season));
        
        private async Task<List<SeasonDataQB>> SeasonDataQB(List<FantasyProsStringParseQB> players, int season)
        {
            List<SeasonDataQB> seasonData = [];
            foreach (var p in players)
            {
                var playerId = await playerService.GetPlayerId(p.Name);
                if(playerId == 0)
                {
                    await playerService.CreatePlayer(new Player { Name = p.Name, Position = Position.QB.ToString(), Active = 1 });
                    logger.Information("New player created: {p}", p.Name);
                    playerId = await playerService.GetPlayerId(p.Name);
                }

                if (p.Games > 0)
                {
                    seasonData.Add(new SeasonDataQB
                    {
                        Season = season,
                        PlayerId = playerId,
                        Name = p.Name,
                        Completions = p.Completions,
                        Attempts = p.Attempts,
                        Yards = p.Yards,
                        TD = p.TD,
                        Int = p.Int,
                        Sacks = p.Sacks,
                        RushingAttempts = p.RushingAttempts,
                        RushingYards = p.RushingYards,
                        RushingTD = p.RushingTD,
                        Fumbles = p.Fumbles,
                        Games = p.Games
                    });
                }
                else
                {
                    logger.Information("{name} does not exist in the Players table", p.Name);
                }
            }
            return seasonData;
        }

        private async Task<List<SeasonDataRB>> SeasonDataRB(List<FantasyProsStringParseRB> players, int season)
        {
            List<SeasonDataRB> seasonData = [];
            foreach (var p in players)
            {
                var playerId = await playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await playerService.CreatePlayer(new Player { Name = p.Name, Position = Position.RB.ToString(), Active = 1 });
                    logger.Information("New player created: {p}", p.Name);
                    playerId = await playerService.GetPlayerId(p.Name);
                }
                if (p.Games > 0)
                {
                    seasonData.Add(new SeasonDataRB
                    {
                        Season = season,
                        PlayerId = playerId,
                        Name = p.Name,
                        RushingAtt = p.RushingAtt,
                        RushingYds = p.RushingYds,
                        RushingTD = p.RushingTD,
                        Receptions = p.Receptions,
                        Targets = p.Targets,
                        Yards = p.Yards,
                        ReceivingTD = p.ReceivingTD,
                        Fumbles = p.Fumbles,
                        Games = p.Games
                    });
                }
                else
                {
                    logger.Information("{name} does not exist in the Players table", p.Name);
                }
            }
            return seasonData;
        }
        private async Task<List<SeasonDataWR>> SeasonDataWR(List<FantasyProsStringParseWR> players, int season)
        {
            List<SeasonDataWR> seasonData = [];
            foreach (var p in players)
            {
                var playerId = await playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await playerService.CreatePlayer(new Player { Name = p.Name, Position = Position.WR.ToString(), Active = 1 });
                    logger.Information("New player created: {p}", p.Name);
                    playerId = await playerService.GetPlayerId(p.Name);
                }
                if (p.Games > 0)
                {
                    seasonData.Add(new SeasonDataWR
                    {
                        Season = season,
                        PlayerId = playerId,
                        Name = p.Name,
                        Receptions = p.Receptions,
                        Targets = p.Targets,
                        Yards = p.Yards,
                        Long = p.Long,
                        TD = p.TD,
                        RushingAtt = p.RushingAtt,
                        RushingYds = p.RushingYds,
                        RushingTD = p.RushingTD,
                        Fumbles = p.Fumbles,
                        Games = p.Games,
                    });
                }
                else
                {
                    logger.Information("{name} does not exist in the Players table", p.Name);
                }
            }
            return seasonData;
        }

        private async Task<List<SeasonDataTE>> SeasonDataTE(List<FantasyProsStringParseTE> players, int season)
        {
            List<SeasonDataTE> seasonData = [];
            foreach (var p in players)
            {
                var playerId = await playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await playerService.CreatePlayer(new Player { Name = p.Name, Position = Position.TE.ToString(), Active = 1 });
                    logger.Information("New player created: {p}", p.Name);
                    playerId = await playerService.GetPlayerId(p.Name);
                }
                if ( p.Games > 0)
                {
                    seasonData.Add(new SeasonDataTE
                    {
                        Season = season,
                        PlayerId = playerId,
                        Name = p.Name,
                        Receptions = p.Receptions,
                        Targets = p.Targets,
                        Yards = p.Yards,
                        Long = p.Long,
                        TD = p.TD,
                        RushingAtt = p.RushingAtt,
                        RushingYds = p.RushingYds,
                        RushingTD = p.RushingTD,
                        Fumbles = p.Fumbles,
                        Games = p.Games
                    });
                }
                else
                {
                    logger.Information("{name} does not exist in the Players table", p.Name);
                }
            }
            return seasonData;
        }

        private async Task<List<SeasonDataDST>> SeasonDataDST(List<FantasyProsStringParseDST> players, int season)
        {
            List<SeasonDataDST> seasonData = [];
            foreach (var p in players)
            {
                var playerId = await playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await playerService.CreatePlayer(new Player { Name = p.Name, Position = Position.DST.ToString(), Active = 1 });
                    logger.Information("New player created: {p}", p.Name);
                    playerId = await playerService.GetPlayerId(p.Name);
                }
                if (p.Games > 0)
                {
                    seasonData.Add(new SeasonDataDST
                    {
                        Season = season,
                        PlayerId = playerId,
                        Name = p.Name,
                        Sacks = p.Sacks,
                        Ints = p.Ints,
                        FumblesRecovered = p.FumblesRecovered,
                        ForcedFumbles = p.ForcedFumbles,
                        DefensiveTD = p.DefensiveTD,
                        Safties = p.Safties,
                        SpecialTD = p.SpecialTD,
                        Games = p.Games
                    });
                }
                else
                {
                    logger.Information("{name} does not exist in the Players table", p.Name);
                }
            }
            return seasonData;
        }

        private async Task<List<SeasonADP>> SeasonADP(List<FantasyProsADP> adp, int season)
        {
            List<SeasonADP> seasonADP = [];
            foreach (var a in adp)
            {
                var playerId = await playerService.GetPlayerId(a.Name);
                if (playerId > 0)
                {
                    var player = await playerService.GetPlayer(playerId);
                    seasonADP.Add(new SeasonADP
                    {
                        Season = season,
                        PlayerId = playerId,
                        Name = a.Name,
                        Position = player.Position,
                        OverallADP = a.OverallADP,
                        PositionADP = a.PositionADP
                    });
                }
            }
            return seasonADP;
        }

        private async Task<List<ScheduleDetails>> ScheduleDetails(List<ProFootballReferenceGameScores> schedule, int season)
        {
            List<ScheduleDetails> scheduleDetails = [];
            foreach (var s in schedule)
            {
                var homeTeamId = 0;
                var awayTeamId = 0;
                if (s.HomeIndicator == "@") {
                    homeTeamId = await playerService.GetTeamIdFromDescription(s.Loser);
                    awayTeamId = await playerService.GetTeamIdFromDescription(s.Winner);
                }
                else
                {
                    homeTeamId = await playerService.GetTeamIdFromDescription(s.Winner);
                    awayTeamId = await playerService.GetTeamIdFromDescription(s.Loser);
                }

                if (awayTeamId > 0 && homeTeamId > 0)
                {
                    scheduleDetails.Add(new ScheduleDetails
                    {
                        Season = season,
                        Week = s.Week,
                        Day = s.Day,
                        Date = s.Date,
                        Time = s.Time,
                        HomeTeamId = homeTeamId,
                        AwayTeamId = awayTeamId
                    });
                }
            }
            return scheduleDetails;
        }
        private string FantasyProsURLFormatter(string position, string year) => string.Format("{0}{1}.php?year={2}", _scraping.FantasyProsBaseURL, position.ToLower(), year);
    }
}

