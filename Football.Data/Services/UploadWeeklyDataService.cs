using Football.Data.Interfaces;
using Football.Data.Models;
using Football.Interfaces;
using Serilog;

namespace Football.Data.Services
{
    public class UploadWeeklyDataService : IUploadWeeklyDataService
    {
        private readonly IScraperService _scraperService;
        private readonly IUploadWeeklyDataRepository _uploadWeeklyDataRepository;
        private readonly IPlayerService _playerService;
        private readonly ILogger _logger;

        private readonly string _xpath =  "//*[@id=\"data\"]/tbody";

        public UploadWeeklyDataService(IScraperService scraperService, IUploadWeeklyDataRepository uploadWeeklyDataRepository, 
            IPlayerService playerService, ILogger logger )
        {
            _scraperService = scraperService;
            _uploadWeeklyDataRepository = uploadWeeklyDataRepository;
            _playerService = playerService;
            _logger = logger;
        }
        public async Task<int> UploadWeeklyQBData(int season, int week)
        {
            var url = _scraperService.FantasyProsURLFormatter("QB", season.ToString(), week.ToString());
            var players = await WeeklyDataQB(_scraperService.ParseFantasyProsQBData(_scraperService.ScrapeData(url, _xpath)), season, week);
            return await _uploadWeeklyDataRepository.UploadWeeklyQBData(players);
        }
        public async Task<int> UploadWeeklyRBData(int season, int week)
        {
            var url = _scraperService.FantasyProsURLFormatter("RB", season.ToString(), week.ToString());
            var players = await WeeklyDataRB(_scraperService.ParseFantasyProsRBData(_scraperService.ScrapeData(url, _xpath)), season, week);
            return await _uploadWeeklyDataRepository.UploadWeeklyRBData(players);
        }
        public async Task<int> UploadWeeklyWRData(int season, int week)
        {
            var url = _scraperService.FantasyProsURLFormatter("WR", season.ToString(), week.ToString());
            var players = await WeeklyDataWR(_scraperService.ParseFantasyProsWRData(_scraperService.ScrapeData(url, _xpath)), season, week);
            return await _uploadWeeklyDataRepository.UploadWeeklyWRData(players);
        }
        public async Task<int> UploadWeeklyTEData(int season, int week)
        {
            var url = _scraperService.FantasyProsURLFormatter("TE", season.ToString(), week.ToString());
            var players = await WeeklyDataTE(_scraperService.ParseFantasyProsTEData(_scraperService.ScrapeData(url, _xpath)), season, week);
            return await _uploadWeeklyDataRepository.UploadWeeklyTEData(players);
        }
        public async Task<int> UploadWeeklyDSTData(int season, int week)
        {
            var url = _scraperService.FantasyProsURLFormatter("DST", season.ToString(), week.ToString());
            var players = await WeeklyDataDST(_scraperService.ParseFantasyProsDSTData(_scraperService.ScrapeData(url, _xpath)), season, week);
            return await _uploadWeeklyDataRepository.UploadWeeklyDSTData(players);
        }

        private async Task<List<WeeklyDataQB>> WeeklyDataQB(List<FantasyProsStringParseQB> players, int season, int week)
        {
            List<WeeklyDataQB> weeklyData = new();
            foreach(var p in players)
            {
                var playerId = await _playerService.GetPlayerId(p.Name);
                if (playerId > 0)
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
                        Fumbles = p.Fumbles
                    });
                }
                else
                {
                    _logger.Information("{name} does not exist in the Players table", p.Name);
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
                if (playerId > 0)
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
                        Fumbles = p.Fumbles
                    });
                }
                else
                {
                    _logger.Information("{name} does not exist in the Players table", p.Name);
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
                if (playerId > 0)
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
                        Fumbles = p.Fumbles
                    });
                }
                else
                {
                    _logger.Information("{name} does not exist in the Players table", p.Name);
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
                if (playerId > 0)
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
                        Fumbles = p.Fumbles
                    });
                }
                else
                {
                    _logger.Information("{name} does not exist in the Players table", p.Name);
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
                if (playerId > 0)
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
                        SpecialTD = p.SpecialTD
                    });
                }
                else
                {
                    _logger.Information("{name} does not exist in the Players table", p.Name);
                }
            }
            return weeklyData;
        }

    }
}
