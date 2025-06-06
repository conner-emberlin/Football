﻿using Football.Models;
using Football.Enums;
using Football.Data.Interfaces;
using Football.Data.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;
using AutoMapper;
using Serilog;

namespace Football.Data.Services
{
    public class UploadSeasonDataService(IScraperService scraperService, IUploadSeasonDataRepository uploadSeasonDataRepository,
         IOptionsMonitor<WeeklyScraping> scraping, IPlayersService playerService, ITeamsService teamsService, IMapper mapper, IOptionsMonitor<Season> season, ILogger logger) : IUploadSeasonDataService
    {
        private readonly WeeklyScraping _scraping = scraping.CurrentValue;
        private readonly Season _season = season.CurrentValue;

        public async Task<int> UploadSeasonQBData(int season)
        {
            var url = FantasyProsURLFormatter(Position.QB.ToString(), season.ToString());
            var ignoreList = await playerService.GetIgnoreList();
            var players = (await SeasonDataQB(scraperService.ParseFantasyProsQBData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season))
                           .Where(p => !ignoreList.Contains(p.PlayerId));
            return await uploadSeasonDataRepository.UploadSeasonQBData(players);
        }
        public async Task<int> UploadSeasonRBData(int season)
        {
            var url = FantasyProsURLFormatter(Position.RB.ToString(), season.ToString());
            var ignoreList = await playerService.GetIgnoreList();
            var players = (await SeasonDataRB(scraperService.ParseFantasyProsRBData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season))
                            .Where(p => !ignoreList.Contains(p.PlayerId));
            return await uploadSeasonDataRepository.UploadSeasonRBData(players);
        }
        public async Task<int> UploadSeasonWRData(int season)
        {
            var url = FantasyProsURLFormatter(Position.WR.ToString(), season.ToString());
            var ignoreList = await playerService.GetIgnoreList();
            var players = (await SeasonDataWR(scraperService.ParseFantasyProsWRData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season))
                            .Where(p => !ignoreList.Contains(p.PlayerId));
            return await uploadSeasonDataRepository.UploadSeasonWRData(players);
        }
        public async Task<int> UploadSeasonTEData(int season)
        {
            var url = FantasyProsURLFormatter(Position.TE.ToString(), season.ToString());
            var ignoreList = await playerService.GetIgnoreList();
            var players = (await SeasonDataTE(scraperService.ParseFantasyProsTEData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season))
                            .Where(p => !ignoreList.Contains(p.PlayerId));
            return await uploadSeasonDataRepository.UploadSeasonTEData(players);
        }
        public async Task<int> UploadCurrentTeams(int season, Position position)
        {
            var url = string.Format("https://www.fantasypros.com/nfl/projections/{0}.php?week=1&scoring=PPR", position.ToString().ToLower());
            var str = scraperService.ScrapeData(url, _scraping.FantasyProsXPath);
            var players = await playerService.GetPlayersByPosition(position, true);
            var playerTeams = await scraperService.ParseFantasyProsPlayerTeam(str, position.ToString());
            List<PlayerTeam> currentTeams = [];
            foreach (var pt in playerTeams)
            {
                var player = players.FirstOrDefault(p => p.Name == pt.Name);
                if (player != null && string.Equals(position.ToString(), player.Position, StringComparison.OrdinalIgnoreCase))
                {
                    currentTeams.Add(new PlayerTeam
                    {
                        PlayerId = player.PlayerId,
                        Name = player.Name,
                        Season = season,
                        Team = pt.Team,
                        TeamId = pt.TeamId
                    });
                }
            }
            return await uploadSeasonDataRepository.UploadCurrentTeams(currentTeams);
        }

        public async Task<int> UploadSchedule(int season)
        {
            var str = scraperService.ScrapeData(_scraping.UploadScheduleURL, _scraping.UploadScheduleXPath);
            var schedules = await scraperService.ParseFantasyProsSeasonSchedule(str);
            return await uploadSeasonDataRepository.UploadSchedule(schedules);
        }
        public async Task<int> UploadScheduleDetails(int season)
        {
            List<ScheduleDetails> schedule = [];
            var seasonGames = await playerService.GetGamesBySeason(season);
            for (int i = 1; i <= seasonGames + 1; i++)
                schedule.AddRange(await ScheduleDetails(await scraperService.ScrapeGameScores(i, false), season));
            return await uploadSeasonDataRepository.UploadScheduleDetails(schedule); ;
        }

        public async Task<int> UploadConsensusProjections(string position)
        {
            var url = string.Format("{0}/{1}/{2}", "https://www.fantasypros.com/nfl/projections", position.ToLower(), ".php?scoring=PPR&week=draft");
            var proj = await ConsensusProjections(scraperService.ParseFantasyProsConsensusProjections(scraperService.ScrapeData(url, _scraping.FantasyProsXPath), position), position);
            return await uploadSeasonDataRepository.UploadConsensusProjections(proj);
        }
        public async Task<int> UploadADP(int season, string position) => await uploadSeasonDataRepository.UploadADP(await SeasonADP(await scraperService.ScrapeADP(position), season));
        
        private async Task<List<SeasonDataQB>> SeasonDataQB(List<FantasyProsStringParseQB> players, int season)
        {
            List<SeasonDataQB> seasonData = [];
            foreach (var p in players)
            {
                var player = await playerService.RetrievePlayer(p.Name, Position.QB, false);
                if (p.Games > 0 && player.Position == Position.QB.ToString())
                {
                    var sd = mapper.Map<SeasonDataQB>(p);
                    sd.Season = season;
                    sd.PlayerId = player.PlayerId;
                    seasonData.Add(sd);
                }
            }
            return seasonData;
        }

        private async Task<List<SeasonDataRB>> SeasonDataRB(List<FantasyProsStringParseRB> players, int season)
        {
            List<SeasonDataRB> seasonData = [];
            foreach (var p in players)
            {
                var player = await playerService.RetrievePlayer(p.Name, Position.RB, false);
                if (p.Games > 0 && player.Position == Position.RB.ToString())
                {
                    var sd = mapper.Map<SeasonDataRB>(p);
                    sd.Season = season;
                    sd.PlayerId = player.PlayerId;
                    seasonData.Add(sd);
                }
            }
            return seasonData;
        }
        private async Task<List<SeasonDataWR>> SeasonDataWR(List<FantasyProsStringParseWR> players, int season)
        {
            List<SeasonDataWR> seasonData = [];
            foreach (var p in players)
            {
                var player = await playerService.RetrievePlayer(p.Name, Position.WR, false);
                if (p.Games > 0 && player.Position == Position.WR.ToString())
                {
                    var sd = mapper.Map<SeasonDataWR>(p);
                    sd.Season = season;
                    sd.PlayerId = player.PlayerId;
                    seasonData.Add(sd);
                }
            }
            return seasonData;
        }

        private async Task<List<SeasonDataTE>> SeasonDataTE(List<FantasyProsStringParseTE> players, int season)
        {
            List<SeasonDataTE> seasonData = [];
            foreach (var p in players)
            {
                var player = await playerService.RetrievePlayer(p.Name, Position.TE, false);
                if ( p.Games > 0 && player.Position == Position.TE.ToString())
                {
                    var sd = mapper.Map<SeasonDataTE>(p);
                    sd.Season = season;
                    sd.PlayerId = player.PlayerId;
                    seasonData.Add(sd);
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

        private async Task<List<ConsensusProjections>> ConsensusProjections(List<FantasyProsConsensusProjections> projections, string position)
        {
            var season = _season.CurrentSeason;
            List<ConsensusProjections> consensusProjections = [];
            foreach (var proj in projections)
            {
                var playerId = await playerService.GetPlayerId(proj.Name);
                if (playerId > 0)
                {
                    var player = await playerService.GetPlayer(playerId);
                    if (string.Equals(player.Position, position, StringComparison.OrdinalIgnoreCase))
                    {
                        consensusProjections.Add(new ConsensusProjections
                        {
                            PlayerId = player.PlayerId,
                            Position = player.Position,
                            Season = season,
                            FantasyPoints = proj.FantasyPoints
                        });
                    }
                }
            }
            return consensusProjections;
        }

        private async Task<List<ScheduleDetails>> ScheduleDetails(List<ProFootballReferenceGameScores> schedule, int season)
        {
            List<ScheduleDetails> scheduleDetails = [];
            foreach (var s in schedule)
            {
                var homeTeamId = 0;
                var awayTeamId = 0;
                if (s.HomeIndicator == "@") 
                {
                    homeTeamId = await teamsService.GetTeamIdFromDescription(s.Loser);
                    awayTeamId = await teamsService.GetTeamIdFromDescription(s.Winner);
                }
                else
                {
                    homeTeamId = await teamsService.GetTeamIdFromDescription(s.Winner);
                    awayTeamId = await teamsService.GetTeamIdFromDescription(s.Loser);
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

