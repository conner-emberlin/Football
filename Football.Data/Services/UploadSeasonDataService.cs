using Football.Models;
using Football.Enums;
using Football.Data.Interfaces;
using Football.Data.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;
using Serilog;

namespace Football.Data.Services
{
    public class UploadSeasonDataService : IUploadSeasonDataService
    {
        private readonly IScraperService _scraperService;
        private readonly IUploadSeasonDataRepository _uploadSeasonDataRepository;
        private readonly ILogger _logger;
        private readonly IPlayersService _playerService;
        private readonly WeeklyScraping _scraping;
        private readonly Season _season;
        public UploadSeasonDataService(IScraperService scraperService, IUploadSeasonDataRepository uploadSeasonDataRepository,
             ILogger logger, IOptionsMonitor<WeeklyScraping> scraping, IPlayersService playerService, IOptionsMonitor<Season> season)
        {
            _scraperService = scraperService;
            _uploadSeasonDataRepository = uploadSeasonDataRepository;
            _logger = logger;
            _scraping = scraping.CurrentValue;
            _playerService = playerService;
            _season = season.CurrentValue;
        }
        public async Task<int> UploadSeasonQBData(int season)
        {
            var url = _scraperService.FantasyProsURLFormatter(Position.QB.ToString(), season.ToString());
            var players = await SeasonDataQB(_scraperService.ParseFantasyProsQBData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await _uploadSeasonDataRepository.UploadSeasonQBData(players, await _playerService.GetIgnoreList());
        }
        public async Task<int> UploadSeasonRBData(int season)
        {
            var url = _scraperService.FantasyProsURLFormatter(Position.RB.ToString(), season.ToString());
            var players = await SeasonDataRB(_scraperService.ParseFantasyProsRBData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await _uploadSeasonDataRepository.UploadSeasonRBData(players, await _playerService.GetIgnoreList());
        }
        public async Task<int> UploadSeasonWRData(int season)
        {
            var url = _scraperService.FantasyProsURLFormatter(Position.WR.ToString(), season.ToString());
            var players = await SeasonDataWR(_scraperService.ParseFantasyProsWRData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await _uploadSeasonDataRepository.UploadSeasonWRData(players, await _playerService.GetIgnoreList());
        }
        public async Task<int> UploadSeasonTEData(int season)
        {
            var url = _scraperService.FantasyProsURLFormatter(Position.TE.ToString(), season.ToString());
            var players = await SeasonDataTE(_scraperService.ParseFantasyProsTEData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await _uploadSeasonDataRepository.UploadSeasonTEData(players, await _playerService.GetIgnoreList());
        }
        public async Task<int> UploadSeasonDSTData(int season)
        {
            var url = _scraperService.FantasyProsURLFormatter(Position.DST.ToString(), season.ToString());
            var players = await SeasonDataDST(_scraperService.ParseFantasyProsDSTData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await _uploadSeasonDataRepository.UploadSeasonDSTData(players);
        }
        public async Task<int> UploadCurrentTeams(int season, string position)
        {
            var url = _scraperService.FantasyProsURLFormatter(position, season.ToString());
            var str = _scraperService.ScrapeData(url, _scraping.FantasyProsXPath);
            var parsedStr = _scraperService.ParseFantasyProsPlayerTeam(str, position);
            foreach (var pt in parsedStr)
            {
                _logger.Information("Getting team for player {playerId}", pt.PlayerId);
                pt.PlayerId = await _playerService.GetPlayerId(pt.Name);
                if(pt.PlayerId == 0)
                {
                    _logger.Information("Player {Name} does not exist in players table", pt.Name);
                }
                pt.Season = season;
            }
            return await _uploadSeasonDataRepository.UploadCurrentTeams(parsedStr);
        }

        public async Task<int> UploadSchedule(int season)
        {
            var str = _scraperService.ScrapeData(_scraping.UploadScheduleURL, _scraping.UploadScheduleXPath);
            var schedules = await _scraperService.ParseFantasyProsSeasonSchedule(str);
            return await _uploadSeasonDataRepository.UploadSchedule(schedules);
        }
        public async Task<int> UploadScheduleDetails(int season)
        {
            var count = 0;
            for (int i = 1; i <= _season.Games + 1; i++)
            {
                var details = await ScheduleDetails(await _scraperService.ScrapeGameScores(i), season);
                count += await _uploadSeasonDataRepository.UploadScheduleDetails(details);
            }
            return count;
        }

        public async Task<int> UploadADP(int season, string position) => await _uploadSeasonDataRepository.UploadADP(await SeasonADP(await _scraperService.ScrapeADP(position), season));
        
        private async Task<List<SeasonDataQB>> SeasonDataQB(List<FantasyProsStringParseQB> players, int season)
        {
            List<SeasonDataQB> seasonData = new();
            foreach (var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
                if(playerId == 0)
                {
                    await _playerService.CreatePlayer(new Player { Name = p.Name, Position = Position.QB.ToString(), Active = 1 });
                    _logger.Information("New player created: {p}", p.Name);
                    playerId = await _playerService.GetPlayerId(p.Name);
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
                    _logger.Information("{name} does not exist in the Players table", p.Name);
                }
            }
            return seasonData;
        }

        private async Task<List<SeasonDataRB>> SeasonDataRB(List<FantasyProsStringParseRB> players, int season)
        {
            List<SeasonDataRB> seasonData = new();
            foreach (var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await _playerService.CreatePlayer(new Player { Name = p.Name, Position = Position.RB.ToString(), Active = 1 });
                    _logger.Information("New player created: {p}", p.Name);
                    playerId = await _playerService.GetPlayerId(p.Name);
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
                    _logger.Information("{name} does not exist in the Players table", p.Name);
                }
            }
            return seasonData;
        }
        private async Task<List<SeasonDataWR>> SeasonDataWR(List<FantasyProsStringParseWR> players, int season)
        {
            List<SeasonDataWR> seasonData = new();
            foreach (var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await _playerService.CreatePlayer(new Player { Name = p.Name, Position = Position.WR.ToString(), Active = 1 });
                    _logger.Information("New player created: {p}", p.Name);
                    playerId = await _playerService.GetPlayerId(p.Name);
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
                    _logger.Information("{name} does not exist in the Players table", p.Name);
                }
            }
            return seasonData;
        }

        private async Task<List<SeasonDataTE>> SeasonDataTE(List<FantasyProsStringParseTE> players, int season)
        {
            List<SeasonDataTE> seasonData = new();
            foreach (var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await _playerService.CreatePlayer(new Player { Name = p.Name, Position = Position.TE.ToString(), Active = 1 });
                    _logger.Information("New player created: {p}", p.Name);
                    playerId = await _playerService.GetPlayerId(p.Name);
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
                    _logger.Information("{name} does not exist in the Players table", p.Name);
                }
            }
            return seasonData;
        }

        private async Task<List<SeasonDataDST>> SeasonDataDST(List<FantasyProsStringParseDST> players, int season)
        {
            List<SeasonDataDST> seasonData = new();
            foreach (var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await _playerService.CreatePlayer(new Player { Name = p.Name, Position = Position.DST.ToString(), Active = 1 });
                    _logger.Information("New player created: {p}", p.Name);
                    playerId = await _playerService.GetPlayerId(p.Name);
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
                    _logger.Information("{name} does not exist in the Players table", p.Name);
                }
            }
            return seasonData;
        }

        private async Task<List<SeasonADP>> SeasonADP(List<FantasyProsADP> adp, int season)
        {
            List<SeasonADP> seasonADP = new();
            foreach (var a in adp)
            {
                var playerId = await _playerService.GetPlayerId(a.Name);
                if (playerId > 0)
                {
                    var player = await _playerService.GetPlayer(playerId);
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
            List<ScheduleDetails> scheduleDetails = new();
            foreach (var s in schedule)
            {
                var homeTeamId = 0;
                var awayTeamId = 0;
                if (s.HomeIndicator == "@") {
                    homeTeamId = await _playerService.GetTeamIdFromDescription(s.Winner);
                    awayTeamId = await _playerService.GetTeamIdFromDescription(s.Loser);
                }
                else
                {
                    homeTeamId = await _playerService.GetTeamIdFromDescription(s.Loser);
                    awayTeamId = await _playerService.GetTeamIdFromDescription(s.Winner);
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
    }
}

