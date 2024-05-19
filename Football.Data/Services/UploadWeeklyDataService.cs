using Football.Enums;
using Football.Models;
using Football.Data.Interfaces;
using Football.Data.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;
using Serilog;
using AutoMapper;

namespace Football.Data.Services
{
    public class UploadWeeklyDataService(IScraperService scraperService, IUploadWeeklyDataRepository uploadWeeklyDataRepository,
        IPlayersService playerService, ILogger logger, IMapper mapper, IOptionsMonitor<WeeklyScraping> scraping) : IUploadWeeklyDataService
    {
        private readonly WeeklyScraping _scraping = scraping.CurrentValue;

        public async Task<int> UploadWeeklyQBData(int season, int week)
        {
            logger.Information("Uploading QB Data for week {0}", week);
            var url = FantasyProsURLFormatter(Position.QB.ToString(), season.ToString(), week.ToString());
            var ignoreList = await playerService.GetIgnoreList();
            var qbs = (await playerService.GetPlayersByPosition(Position.QB)).Select(p => p.PlayerId);
            var players = (await WeeklyDataQB(scraperService.ParseFantasyProsQBData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season, week))
                            .Where(p => !ignoreList.Contains(p.PlayerId) && qbs.Contains(p.PlayerId));
            var added = await uploadWeeklyDataRepository.UploadWeeklyQBData(players);
            logger.Information("QB upload complete. {0} records added", added);
            return added;
        }
        public async Task<int> UploadWeeklyRBData(int season, int week)
        {
            logger.Information("Uploadeding RB Data for week {0", week);
            var url = FantasyProsURLFormatter(Position.RB.ToString(), season.ToString(), week.ToString());
            var ignoreList = await playerService.GetIgnoreList();
            var rbs = (await playerService.GetPlayersByPosition(Position.RB)).Select(p => p.PlayerId);
            var players = (await WeeklyDataRB(scraperService.ParseFantasyProsRBData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season, week))
                            .Where(p => !ignoreList.Contains(p.PlayerId) && rbs.Contains(p.PlayerId));
            var added = await uploadWeeklyDataRepository.UploadWeeklyRBData(players);
            logger.Information("RB upload complete. {0} records added", added);
            return added;
        }
        public async Task<int> UploadWeeklyWRData(int season, int week)
        {
            logger.Information("Uploading WR Data for week {0}", week);
            var url = FantasyProsURLFormatter(Position.WR.ToString(), season.ToString(), week.ToString());
            var ignoreList = await playerService.GetIgnoreList();
            var wrs = (await playerService.GetPlayersByPosition(Position.WR)).Select(p => p.PlayerId);
            var players = (await WeeklyDataWR(scraperService.ParseFantasyProsWRData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season, week))
                            .Where(p => !ignoreList.Contains(p.PlayerId) && wrs.Contains(p.PlayerId));
            var added = await uploadWeeklyDataRepository.UploadWeeklyWRData(players);
            logger.Information("WR upload complete. {0} records added", added);
            return added;
        }
        public async Task<int> UploadWeeklyTEData(int season, int week)
        {
            logger.Information("Uploading TE Data for week {0}", week);
            var url = FantasyProsURLFormatter(Position.TE.ToString(), season.ToString(), week.ToString());
            var ignoreList = await playerService.GetIgnoreList();
            var tes = (await playerService.GetPlayersByPosition(Position.TE)).Select(p => p.PlayerId);
            var players = (await WeeklyDataTE(scraperService.ParseFantasyProsTEData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season, week))
                            .Where(p => !ignoreList.Contains(p.PlayerId) && tes.Contains(p.PlayerId));
            var added = await uploadWeeklyDataRepository.UploadWeeklyTEData(players);
            logger.Information("TE upload complete. {0} records added", added);
            return added;
        }
        public async Task<int> UploadWeeklyDSTData(int season, int week)
        {
            logger.Information("Uploading DST Data for week {0}", week);
            var url = FantasyProsURLFormatter(Position.DST.ToString(), season.ToString(), week.ToString());
            var players = await WeeklyDataDST(scraperService.ParseFantasyProsDSTData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season, week);
            var added = await uploadWeeklyDataRepository.UploadWeeklyDSTData(players);
            logger.Information("DST upload complete. {0} records added", added);
            return added;
        }

        public async Task<int> UploadWeeklyKData(int season, int week)
        {
            logger.Information("Uploading K Data for week {0}", week);
            var url = FantasyProsURLFormatter(Position.K.ToString(), season.ToString(), week.ToString());
            var players = await WeeklyDataK(scraperService.ParseFantasyProsKData(scraperService.ScrapeData(url, _scraping.FantasyProsXPath)), season, week);
            var added = await uploadWeeklyDataRepository.UploadWeeklyKData(players);
            logger.Information("K upload complete. {0} records added", added);
            return added;
        }

        public async Task<int> UploadWeeklyRedZoneRB(int season, int week, int yardline)
        {
            logger.Information("Uploading RB Redzone data for week {0}", week);
            var url = RedZoneURL(Position.RB.ToString(), season, week, yardline);
            var ignoreList = await playerService.GetIgnoreList();
            var rbs = (await playerService.GetPlayersByPosition(Position.RB)).Select(p => p.PlayerId);
            var players = (await WeeklyRedZoneRB(scraperService.ParseFantasyProsRedZoneRB(scraperService.ScrapeData(url, _scraping.RedZoneXPath)), season, week, yardline))
                            .Where(p => !ignoreList.Contains(p.PlayerId) && rbs.Contains(p.PlayerId));
            var added = await uploadWeeklyDataRepository.UploadWeeklyRedZoneRB(players);
            logger.Information("RB Redzone upload complete. {0} records added", added);
            return added;
        }

        public async Task<int> UploadWeeklyGameResults(int season, int week)
        {
            logger.Information("Uploading Game Results for week {0}", week);
            var results = await GameResult(await scraperService.ScrapeGameScores(week), season);
            var added = await uploadWeeklyDataRepository.UploadWeeklyGameResults(results);
            logger.Information("Game Result upload complete. {0} records added", added);
            return added;
        }
        public async Task<int> UploadWeeklyRosterPercentages(int season, int week, string position)
        {
            logger.Information("Uploading week {0} Roster Percentages for position {1}", week, position);
            var url = FantasyProsURLFormatter(position.ToString(), season.ToString(), week.ToString());
            var data = scraperService.ParseFantasyProsRosterPercent(scraperService.ScrapeData(url, _scraping.FantasyProsXPath), position);
            var ignoreList = await playerService.GetIgnoreList();
            var rosterPercentages = (await WeeklyRosterPercent(data, season, week)).Where(r => !ignoreList.Contains(r.PlayerId));
            var added = await uploadWeeklyDataRepository.UploadWeeklyRosterPercentages(rosterPercentages);
            logger.Information("Roster Percentage upload complete. {0} records added", added);
            return added;
        }

        public async Task<int> UploadWeeklySnapCounts(int season, int week, string position)
        {
            logger.Information("Uploading week {0} Snap Counts for position {1}", week, position);
            var data = await scraperService.ScrapeSnapCounts(position, week);
            var snapCounts = await SnapCount(data, season, week);
            var added = await uploadWeeklyDataRepository.UploadWeeklySnapCounts(snapCounts);
            logger.Information("Snap Count upload complete. {0} records added", added);
            return added;
        }

        private async Task<List<WeeklyRosterPercent>> WeeklyRosterPercent(List<FantasyProsRosterPercent> rosterPercents, int season, int week)
        {
            List<WeeklyRosterPercent> rosterPercentages = [];
            foreach (var rp in rosterPercents.Where(rp => rp.RosterPercent > 0.0))
            {
                var playerId = await playerService.GetPlayerId(rp.Name);
                if (playerId > 0 && !rosterPercentages.Any(rp => rp.PlayerId == playerId))
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
            List<WeeklyDataQB> weeklyData = [];
            foreach(var p in players)
            {
                var player = await playerService.RetrievePlayer(p.Name, Position.QB);
                if (p.Games > 0 && player.Position == Position.QB.ToString())
                {
                    var wd= mapper.Map<WeeklyDataQB>(p);
                    wd.Season = season;
                    wd.Week = week;
                    wd.PlayerId = player.PlayerId;
                    weeklyData.Add(wd);
                }
            }
            return weeklyData;
        }

        private async Task<List<WeeklyDataRB>> WeeklyDataRB(List<FantasyProsStringParseRB> players, int season, int week)
        {
            List<WeeklyDataRB> weeklyData = [];
            foreach (var p in players)
            {
                var player = await playerService.RetrievePlayer(p.Name, Position.RB);
                if (p.Games > 0 && player.Position == Position.RB.ToString())
                {
                    var wd = mapper.Map<WeeklyDataRB>(p);
                    wd.Season = season;
                    wd.Week = week;
                    wd.PlayerId = player.PlayerId;
                    weeklyData.Add(wd);
                }
            }
            return weeklyData;
        }

        private async Task<List<WeeklyRedZoneRB>> WeeklyRedZoneRB(List<FantasyProsStringParseRB> players, int season, int week, int yardline)
        {
            List<WeeklyRedZoneRB> weeklyData = [];
            foreach (var p in players)
            {
                var playerId = await playerService.GetPlayerId(p.Name);
                if (playerId == 0) break;
                var player = await playerService.GetPlayer(playerId);
                if (p.Games > 0 && player.Position == Position.RB.ToString())
                {
                    weeklyData.Add(new WeeklyRedZoneRB
                    {
                        Season = season,
                        Week = week,
                        PlayerId = playerId,
                        Name = p.Name,
                        Yardline = yardline,
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
            }
            return weeklyData;
        }
        private async Task<List<WeeklyDataWR>> WeeklyDataWR(List<FantasyProsStringParseWR> players, int season, int week)
        {
            List<WeeklyDataWR> weeklyData = [];
            foreach (var p in players)
            {
                var player = await playerService.RetrievePlayer(p.Name, Position.WR);
                if (p.Games > 0 && player.Position == Position.WR.ToString())
                {
                    var wd = mapper.Map<WeeklyDataWR>(p);
                    wd.Season = season;
                    wd.Week = week;
                    wd.PlayerId = player.PlayerId;
                    weeklyData.Add(wd);
                }
            }
            return weeklyData;
        }

        private async Task<List<WeeklyDataTE>> WeeklyDataTE(List<FantasyProsStringParseTE> players, int season, int week)
        {
            List<WeeklyDataTE> weeklyData = [];
            foreach (var p in players)
            {
                var player = await playerService.RetrievePlayer(p.Name, Position.TE);
                if (p.Games > 0 && player.Position == Position.TE.ToString())
                {
                    var wd = mapper.Map<WeeklyDataTE>(p);
                    wd.Season = season;
                    wd.Week = week;
                    wd.PlayerId = player.PlayerId;
                    weeklyData.Add(wd);
                }
            }
            return weeklyData;
        }

        private async Task<List<WeeklyDataDST>> WeeklyDataDST(List<FantasyProsStringParseDST> players, int season, int week)
        {
            List<WeeklyDataDST> weeklyData = [];
            foreach (var p in players)
            {
                var playerId = await playerService.GetPlayerId(p.Name);
                if (playerId == 0)
                    throw new Exception("Invalid DST");
                if (p.Games > 0)
                {
                    var wd = mapper.Map<WeeklyDataDST>(p);
                    wd.Season = season;
                    wd.Week = week;
                    wd.PlayerId = playerId;
                    weeklyData.Add(wd);
                }
            }
            return weeklyData;
        }

        private async Task<List<WeeklyDataK>> WeeklyDataK(List<FantasyProsStringParseK> players, int season, int week)
        {
            List<WeeklyDataK> weeklyData = [];
            foreach (var p in players)
            {
                var player = await playerService.RetrievePlayer(p.Name, Position.K);
                if (p.Games > 0 && player.Position == Position.K.ToString())
                {
                    var wd = mapper.Map<WeeklyDataK>(p);
                    wd.Season = season;
                    wd.Week = week;
                    wd.PlayerId = player.PlayerId;
                    weeklyData.Add(wd);
                }
            }
            return weeklyData;
        }

        private async Task<List<GameResult>> GameResult(List<ProFootballReferenceGameScores> games, int season)
        {
            List<GameResult> results = [];
            foreach (var g in games)
            {
                var winnerId = await playerService.GetTeamIdFromDescription(g.Winner);
                var loserId = await playerService.GetTeamIdFromDescription(g.Loser);
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

        private async Task<List<SnapCount>> SnapCount(List<FantasyProsSnapCount> snaps, int season, int week)
        {
            List<SnapCount> snapCounts = [];
            foreach (var s in snaps)
            {
                var playerId = await playerService.GetPlayerId(s.Name);
                if (playerId > 0 && s.Snaps > 0)
                {
                    var player = await playerService.GetPlayer(playerId);
                    snapCounts.Add(new SnapCount
                    {
                        Season = season,
                        Week = week,
                        PlayerId = playerId,
                        Name = s.Name,
                        Position = player.Position,
                        Snaps = s.Snaps
                    });
                }
            }
            return snapCounts;
        }
        private string FantasyProsURLFormatter(string position, string year, string week) => string.Format("{0}{1}.php?year={2}&week={3}&range=week", _scraping.FantasyProsBaseURL, position.ToLower(), year, week);
        private string RedZoneURL(string position, int season, int week, int yardline) => string.Format("{0}{1}.php?year={2}&week={3}&yardline={4}&range=week", _scraping.RedZoneURL, position.ToLower(), season, week, yardline);
    }
}
