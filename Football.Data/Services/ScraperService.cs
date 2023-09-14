using Football.Data.Interfaces;
using Football.Data.Models;
using Football.Players.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Serilog;
using System.IO;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace Football.Data.Services
{
    public class ScraperService : IScraperService
    {
        private readonly WeeklyScraping _scraping;
        private readonly IPlayersService _playersService;
        private readonly ILogger _logger;
        public ScraperService(IOptionsMonitor<WeeklyScraping> scraping, IPlayersService playersService, ILogger logger)
        {
            _scraping = scraping.CurrentValue;
            _playersService = playersService;
            _logger = logger;
        }
        public string FantasyProsURLFormatter(string position, string year, string week)
        {
            return String.Format("{0}{1}.php?year={2}&week={3}&range=week", _scraping.FantasyProsBaseURL, position.ToLower(), year, week);
        }
        public string FantasyProsURLFormatter(string position, string year)
        {
            return String.Format("{0}{1}.php?year={2}", _scraping.FantasyProsBaseURL, position.ToLower(), year);
        }

        public string[] ScrapeData(string url, string xpath)
        {
            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            var data = doc.DocumentNode.SelectNodes(xpath)[0].InnerText;
            string[] strings = data.Split(new string[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.RemoveEmptyEntries);
            return strings;
        }
        public List<FantasyProsStringParseWR> ParseFantasyProsWRData(string[] strings)
        {
            List<FantasyProsStringParseWR> players = new();
            for (int i = 0; i < strings.Length - 15; i += 15)
            {
                FantasyProsStringParseWR parse = new()
                {
                    Name = FormatName(strings[i]),
                    Receptions = double.Parse(Regex.Match(strings[i].Trim(), @"\d+$", RegexOptions.RightToLeft).Value),
                    Targets = double.Parse(strings[i + 1].Trim()),
                    Yards = double.Parse(strings[i + 2].Trim()),
                    Long = double.Parse(strings[i + 4].Trim()),
                    TD = double.Parse(strings[i + 6].Trim()),
                    RushingAtt = double.Parse(strings[i + 7].Trim()),
                    RushingYds = double.Parse(strings[i + 8].Trim()),
                    RushingTD = double.Parse(strings[i + 9].Trim()),
                    Fumbles = double.Parse(strings[i + 10].Trim()),
                    Games = double.Parse(strings[i + 11].Trim()),
                };
                players.Add(parse);
            }
            return players;
        }
        public List<FantasyProsStringParseRB> ParseFantasyProsRBData(string[] strings)
        {
            List<FantasyProsStringParseRB> players = new();
            for (int i = 0; i < strings.Length - 16; i += 16)
            {
                FantasyProsStringParseRB parse = new()
                {
                    Name = FormatName(strings[i]),
                    RushingAtt = double.Parse(Regex.Match(strings[i].Trim(), @"\d+$", RegexOptions.RightToLeft).Value),
                    RushingYds = double.Parse(strings[i + 1].Trim()),
                    RushingTD = double.Parse(strings[i + 5].Trim()),
                    Receptions = double.Parse(strings[i + 6].Trim()),
                    Targets = double.Parse(strings[i + 7].Trim()),
                    Yards = double.Parse(strings[i + 8].Trim()),
                    ReceivingTD = double.Parse(strings[i + 10].Trim()),
                    Fumbles = double.Parse(strings[i + 11].Trim()),
                    Games = double.Parse(strings[i + 12].Trim()),
                };
                players.Add(parse);
            }
            return players;
        }

        public List<FantasyProsStringParseQB> ParseFantasyProsQBData(string[] strings)
        {
            List<FantasyProsStringParseQB> players = new();
            for (int i = 0; i < strings.Length - 16; i += 16)
            {
                FantasyProsStringParseQB parse = new()
                {
                    Name = FormatName(strings[i]),
                    Completions = double.Parse(Regex.Match(strings[i].Trim(), @"\d+$", RegexOptions.RightToLeft).Value),
                    Attempts = double.Parse(strings[i + 1].Trim()),
                    Yards = double.Parse(strings[i + 3].Trim()),
                    TD = double.Parse(strings[i + 5].Trim()),
                    Int = double.Parse(strings[i + 6].Trim()),
                    Sacks = double.Parse(strings[i + 7].Trim()),
                    RushingAttempts = double.Parse(strings[i + 8].Trim()),
                    RushingYards = double.Parse(strings[i + 9].Trim()),
                    RushingTD = double.Parse(strings[i + 10].Trim()),
                    Fumbles = double.Parse(strings[i + 11].Trim()),
                    Games = double.Parse(strings[i + 12].Trim()),
                };
                players.Add(parse);
            }
            return players;
        }
        public List<FantasyProsStringParseTE> ParseFantasyProsTEData(string[] strings)
        {
            List<FantasyProsStringParseTE> players = new();
            for (int i = 0; i < strings.Length - 15; i += 15)
            {
                FantasyProsStringParseTE parse = new()
                {
                    Name = FormatName(strings[i]),
                    Receptions = double.Parse(Regex.Match(strings[i].Trim(), @"\d+$", RegexOptions.RightToLeft).Value),
                    Targets = double.Parse(strings[i + 1].Trim()),
                    Yards = double.Parse(strings[i + 2].Trim()),
                    Long = double.Parse(strings[i + 4].Trim()),
                    TD = double.Parse(strings[i + 6].Trim()),
                    RushingAtt = double.Parse(strings[i + 7].Trim()),
                    RushingYds = double.Parse(strings[i + 8].Trim()),
                    RushingTD = double.Parse(strings[i + 9].Trim()),
                    Fumbles = double.Parse(strings[i + 10].Trim()),
                    Games = double.Parse(strings[i + 11].Trim()),
                };
                players.Add(parse);
            }
            return players;
        }
        public List<FantasyProsStringParseDST> ParseFantasyProsDSTData(string[] strings)
        {
            List<FantasyProsStringParseDST> players = new();
            for (int i = 0; i < strings.Length - 11; i += 11)
            {
                FantasyProsStringParseDST parse = new()
                {
                    Name = FormatName(strings[i]),
                    Sacks = double.Parse(Regex.Match(strings[i].Trim(), @"\d+$", RegexOptions.RightToLeft).Value),
                    Ints = double.Parse(strings[i + 1].Trim()),
                    FumblesRecovered = double.Parse(strings[i + 2].Trim()),
                    ForcedFumbles = double.Parse(strings[i + 3].Trim()),
                    DefensiveTD = double.Parse(strings[i + 4].Trim()),
                    Safties = double.Parse(strings[i + 5].Trim()),
                    SpecialTD = double.Parse(strings[i + 6].Trim()),
                    Games = double.Parse(strings[i + 7].Trim()),
                };
                players.Add(parse);
            }
            return players;
        }

        public async Task<int> DownloadHeadShots(string position)
        {
            var web = new HtmlWeb();
            List<string> urls = new();
            var count = 0;
            var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var activePlayers = (await _playersService.GetAllPlayers()).Where(p => p.Active == 1 && p.Position == position).ToList();
            foreach (var player in activePlayers)
            {
                _logger.Information("Getting url for {p}", player.Name);
                var name = Regex.Replace(player.Name, @"[^\w\s]", string.Empty).Trim();
                name = Regex.Replace(name, @"Jr", string.Empty).Trim();
                name = Regex.Replace(name, @"III", string.Empty).Trim();
                name = Regex.Replace(name, @"II", string.Empty).Trim();
                name = Regex.Replace(name, @"[\s]", "-").Trim();
                name = name.ToLower().Trim();
                var baseUrl = string.Format("{0}{1}.php", "https://www.fantasypros.com/nfl/players/", name);
                HtmlDocument doc = web.Load(baseUrl);
                if (doc != null)
                {
                    var xpath = "//*[@id=\"main-container\"]/div/nav/picture/img";
                    var tempdata = doc.DocumentNode.SelectNodes(xpath);
                    if (tempdata != null)
                    {
                        var url = linkParser.Match(tempdata[0].OuterHtml).Value;
                        HttpClient client = new();
                        var res = await client.GetAsync(url);
                        byte[] bytes = await res.Content.ReadAsByteArrayAsync();
                        MemoryStream memoryStream = new(bytes);
                        var fileName = @"C:\NFLData\Headshots\" + player.PlayerId.ToString() + ".png";
                        if (!System.IO.File.Exists(fileName))
                        {
                            using var fs = new FileStream(fileName, FileMode.Create);
                            memoryStream.WriteTo(fs);
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        private string FormatName(string name)
        {
            name = Regex.Replace(name, @"[\d-]", string.Empty);
            name = Regex.Replace(name, @"\(.*\)", string.Empty).Trim();
            return name;
        }
    }
}
