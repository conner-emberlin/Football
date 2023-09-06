using Football.Data.Interfaces;
using Football.Data.Models;
using Football.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;

namespace Football.Data.Services
{
    public class UploadSeasonDataService : IUploadSeasonDataService
    {
        private readonly IScraperService _scraperService;
        private readonly IUploadSeasonDataRepository _uploadSeasonDataRepository;
        private readonly ILogger _logger;
        private readonly IPlayerService _playerService;
        private readonly WeeklyScraping _scraping;
        public UploadSeasonDataService(IScraperService scraperService, IUploadSeasonDataRepository uploadSeasonDataRepository,
             ILogger logger, IOptionsMonitor<WeeklyScraping> scraping, IPlayerService playerService)
        {
            _scraperService = scraperService;
            _uploadSeasonDataRepository = uploadSeasonDataRepository;
            _logger = logger;
            _scraping = scraping.CurrentValue;
            _playerService = playerService; 
        }
        public async Task<int> UploadSeasonQBData(int season)
        {
            var url = _scraperService.FantasyProsURLFormatter("QB", season.ToString());
            var players = await SeasonDataQB(_scraperService.ParseFantasyProsQBData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await _uploadSeasonDataRepository.UploadSeasonQBData(players);
        }
        public async Task<int> UploadSeasonRBData(int season)
        {
            var url = _scraperService.FantasyProsURLFormatter("RB", season.ToString());
            var players = await SeasonDataRB(_scraperService.ParseFantasyProsRBData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await _uploadSeasonDataRepository.UploadSeasonRBData(players);
        }
        public async Task<int> UploadSeasonWRData(int season)
        {
            var url = _scraperService.FantasyProsURLFormatter("WR", season.ToString());
            var players = await SeasonDataWR(_scraperService.ParseFantasyProsWRData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await _uploadSeasonDataRepository.UploadSeasonWRData(players);
        }
        public async Task<int> UploadSeasonTEData(int season)
        {
            var url = _scraperService.FantasyProsURLFormatter("TE", season.ToString());
            var players = await SeasonDataTE(_scraperService.ParseFantasyProsTEData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await _uploadSeasonDataRepository.UploadSeasonTEData(players);
        }
        public async Task<int> UploadSeasonDSTData(int season)
        {
            var url = _scraperService.FantasyProsURLFormatter("DST", season.ToString());
            var players = await SeasonDataDST(_scraperService.ParseFantasyProsDSTData(_scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season);
            return await _uploadSeasonDataRepository.UploadSeasonDSTData(players);
        }

        private async Task<List<SeasonDataQB>> SeasonDataQB(List<FantasyProsStringParseQB> players, int season)
        {
            List<SeasonDataQB> seasonData = new();
            foreach (var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
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

    }
}

