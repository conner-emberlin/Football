using Football.Enums;
using Football.Data.Interfaces;
using Football.Data.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;
using Serilog;

namespace Football.Data.Services
{
    public class UploadWeeklyDataService : IUploadWeeklyDataService
    {
        private readonly IScraperService _scraperService;
        private readonly IUploadWeeklyDataRepository _uploadWeeklyDataRepository;
        private readonly IPlayersService _playerService;
        private readonly ILogger _logger;
        private readonly WeeklyScraping _scraping;
        public UploadWeeklyDataService(IScraperService scraperService, IUploadWeeklyDataRepository uploadWeeklyDataRepository, 
            IPlayersService playerService, ILogger logger, IOptionsMonitor<WeeklyScraping> scraping)
        {
            _scraperService = scraperService;
            _uploadWeeklyDataRepository = uploadWeeklyDataRepository;
            _playerService = playerService;
            _logger = logger;
            _scraping = scraping.CurrentValue;
        }
        public async Task<int> UploadWeeklyQBData(int season, int week)
        {
            var url = _scraperService.FantasyProsURLFormatter(PositionEnum.QB.ToString(), season.ToString(), week.ToString());
            var players = await WeeklyDataQB(_scraperService.ParseFantasyProsQBData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season, week);
            return await _uploadWeeklyDataRepository.UploadWeeklyQBData(players, await _playerService.GetIgnoreList());
        }
        public async Task<int> UploadWeeklyRBData(int season, int week)
        {
            var url = _scraperService.FantasyProsURLFormatter(PositionEnum.RB.ToString(), season.ToString(), week.ToString());
            var players = await WeeklyDataRB(_scraperService.ParseFantasyProsRBData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season, week);
            return await _uploadWeeklyDataRepository.UploadWeeklyRBData(players, await _playerService.GetIgnoreList());
        }
        public async Task<int> UploadWeeklyWRData(int season, int week)
        {
            var url = _scraperService.FantasyProsURLFormatter(PositionEnum.WR.ToString(), season.ToString(), week.ToString());
            var players = await WeeklyDataWR(_scraperService.ParseFantasyProsWRData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season, week);
            return await _uploadWeeklyDataRepository.UploadWeeklyWRData(players, await _playerService.GetIgnoreList());
        }
        public async Task<int> UploadWeeklyTEData(int season, int week)
        {
            var url = _scraperService.FantasyProsURLFormatter(PositionEnum.TE.ToString(), season.ToString(), week.ToString());
            var players = await WeeklyDataTE(_scraperService.ParseFantasyProsTEData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season, week);
            return await _uploadWeeklyDataRepository.UploadWeeklyTEData(players, await _playerService.GetIgnoreList());
        }
        public async Task<int> UploadWeeklyDSTData(int season, int week)
        {
            var url = _scraperService.FantasyProsURLFormatter(PositionEnum.DST.ToString(), season.ToString(), week.ToString());
            var players = await WeeklyDataDST(_scraperService.ParseFantasyProsDSTData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season, week);
            return await _uploadWeeklyDataRepository.UploadWeeklyDSTData(players);
        }
        public async Task<int> UploadWeeklyGameResults(int season, int week)
        {
            var results = await GameResult(await _scraperService.ScrapeGameScores(week), season);
            return await _uploadWeeklyDataRepository.UploadWeeklyGameResults(results);
        }
        public async Task<int> UploadWeeklyRosterPercentages(int season, int week, string position)
        {
            var url = _scraperService.FantasyProsURLFormatter(position.ToString(), season.ToString(), week.ToString());
            var data = _scraperService.ParseFantasyProsRosterPercent(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath), position);
            var rosterPercentages = await WeeklyRosterPercent(data, season, week);
            return await _uploadWeeklyDataRepository.UploadWeeklyRosterPercentages(rosterPercentages);
        }

        private async Task<List<WeeklyRosterPercent>> WeeklyRosterPercent(List<FantasyProsRosterPercent> rosterPercents, int season, int week)
        {
            List<WeeklyRosterPercent> rosterPercentages = new();
            foreach (var rp in rosterPercents)
            {
                var playerId = await _playerService.GetPlayerId(rp.Name);
                if (playerId > 0)
                {
                    rosterPercentages.Add(new WeeklyRosterPercent
                    {
                        Season = season,
                        Week = week,
                        PlayerId = playerId,
                        Name = rp.Name,
                        RosterPercent = rp.RosterPercent
                        
                    });
                }
            }
            return rosterPercentages;
        }
        private async Task<List<WeeklyDataQB>> WeeklyDataQB(List<FantasyProsStringParseQB> players, int season, int week)
        {
            List<WeeklyDataQB> weeklyData = new();
            foreach(var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await _playerService.CreatePlayer(new Player { Name = p.Name, Position = PositionEnum.QB.ToString(), Active = 1 });
                    _logger.Information("New player created: {p}", p.Name);
                    playerId = await _playerService.GetPlayerId(p.Name);
                }
                if (p.Games > 0)
                {
                    weeklyData.Add(new WeeklyDataQB
                    {
                        Season = season,
                        Week = week,
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
                    _logger.Information("{name} did not play in week {week}", p.Name, week);
                }
            }
            return weeklyData;
        }

        private async Task<List<WeeklyDataRB>> WeeklyDataRB(List<FantasyProsStringParseRB> players, int season, int week)
        {
            List<WeeklyDataRB> weeklyData = new();
            foreach (var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await _playerService.CreatePlayer(new Player { Name = p.Name, Position = PositionEnum.RB.ToString(), Active = 1 });
                    _logger.Information("New player created: {p}", p.Name);
                    playerId = await _playerService.GetPlayerId(p.Name);
                }
                if (p.Games > 0)
                {
                    weeklyData.Add(new WeeklyDataRB
                    {
                        Season = season,
                        Week = week,
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
                    _logger.Information("{name} did not play in week {week}", p.Name, week);
                }
            }
            return weeklyData;
        }
        private async Task<List<WeeklyDataWR>> WeeklyDataWR(List<FantasyProsStringParseWR> players, int season, int week)
        {
            List<WeeklyDataWR> weeklyData = new();
            foreach (var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await _playerService.CreatePlayer(new Player { Name = p.Name, Position = PositionEnum.WR.ToString(), Active = 1 });
                    _logger.Information("New player created: {p}", p.Name);
                    playerId = await _playerService.GetPlayerId(p.Name);
                }
                if (p.Games > 0)
                {
                    weeklyData.Add(new WeeklyDataWR
                    {
                        Season = season,
                        Week = week,
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
                    _logger.Information("{name} did not play in week {week}", p.Name, week);
                }
            }
            return weeklyData;
        }

        private async Task<List<WeeklyDataTE>> WeeklyDataTE(List<FantasyProsStringParseTE> players, int season, int week)
        {
            List<WeeklyDataTE> weeklyData = new();
            foreach (var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await _playerService.CreatePlayer(new Player { Name = p.Name, Position = PositionEnum.TE.ToString(), Active = 1 });
                    _logger.Information("New player created: {p}", p.Name);
                    playerId = await _playerService.GetPlayerId(p.Name);
                }
                if (p.Games > 0)
                {
                    weeklyData.Add(new WeeklyDataTE
                    {
                        Season = season,
                        Week = week,
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
                    _logger.Information("{name} did not play in week {week}", p.Name, week);
                }
            }
            return weeklyData;
        }

        private async Task<List<WeeklyDataDST>> WeeklyDataDST(List<FantasyProsStringParseDST> players, int season, int week)
        {
            List<WeeklyDataDST> weeklyData = new();
            foreach (var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                {
                    await _playerService.CreatePlayer(new Player { Name = p.Name, Position = PositionEnum.DST.ToString(), Active = 1 });
                    _logger.Information("New player created: {p}", p.Name);
                    playerId = await _playerService.GetPlayerId(p.Name);
                }
                if (p.Games > 0)
                {
                    weeklyData.Add(new WeeklyDataDST
                    {
                        Season = season,
                        Week = week,
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
                    _logger.Information("{name} did not play in week {week}", p.Name, week);
                }
            }
            return weeklyData;
        }

        private async Task<List<GameResult>> GameResult(List<ProFootballReferenceGameScores> games, int season)
        {
            List<GameResult> results = new();
            foreach (var g in games)
            {
                var winnerId = await _playerService.GetTeamIdFromDescription(g.Winner);
                var loserId = await _playerService.GetTeamIdFromDescription(g.Loser);
                results.Add(new GameResult
                {
                    Season = season,
                    WinnerId = winnerId,
                    LoserId = loserId,
                    HomeTeamId = g.HomeIndicator == "@" ? loserId : winnerId,
                    AwayTeamId = g.HomeIndicator == "@" ? winnerId : loserId,
                    Week = g.Week,
                    Day = g.Day,
                    Date = g.Date,
                    Time = g.Time,
                    Winner = g.Winner,
                    Loser = g.Loser,
                    WinnerPoints = g.WinnerPoints,
                    LoserPoints = g.LoserPoints,
                    WinnerYards = g.WinnerYards,
                    LoserYards = g.LoserYards
                });
            }
            return results;
        }

    }
}
