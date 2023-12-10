using Football.Models;
using Football.Data.Interfaces;
using Football.Data.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace Football.Data.Services
{
    public class ScraperService(IHttpClientFactory clientFactory, IOptionsMonitor<WeeklyScraping> scraping, IPlayersService playersService, ILogger logger, IOptionsMonitor<Season> season) : IScraperService
    {
        private readonly WeeklyScraping _scraping = scraping.CurrentValue;
        private readonly Season _season = season.CurrentValue;

        private static readonly string[] separator = ["\r\n", "\r", "\n"];
        private static readonly string[] homeSeparator = ["@", "vs"];
        private static readonly string[] dataSeparator = [ "data-stat" ];

        public string[] ScrapeData(string url, string xpath)
        {
            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            return doc.DocumentNode.SelectNodes(xpath)[0].InnerText.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public List<FantasyProsRosterPercent> ParseFantasyProsRosterPercent(string[] strings, string position)
        {
            List<FantasyProsRosterPercent> rosterPercent = [];
            var len = GetFantasyProsTableLength(position);
            for(int i = 0; i < strings.Length - len; i+=len)
            {
                rosterPercent.Add(new FantasyProsRosterPercent
                {
                    Name = FormatName(strings[i]),
                    RosterPercent = double.Parse(strings[i + len - 1][0 ..strings[i + len - 1].IndexOf("%")])
                }); 
            }
            return rosterPercent;
        }
        public List<FantasyProsStringParseWR> ParseFantasyProsWRData(string[] strings)
        {
            List<FantasyProsStringParseWR> players = [];
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
            List<FantasyProsStringParseRB> players = [];
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
            List<FantasyProsStringParseQB> players = [];
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
            List<FantasyProsStringParseTE> players = [];
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
            List<FantasyProsStringParseDST> players = [];
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

        public List<FantasyProsStringParseK> ParseFantasyProsKData(string[] strings)
        {
            List<FantasyProsStringParseK> players = [];
            for (int i = 0; i < strings.Length - 15; i += 15)
            {
                FantasyProsStringParseK parse = new()
                {
                    Name = FormatName(strings[i]),
                    FieldGoals = double.Parse(Regex.Match(strings[i].Trim(), @"\d+$", RegexOptions.RightToLeft).Value),
                    FieldGoalAttempts = double.Parse(strings[i+1].Trim()),
                    OneNineteen = double.Parse(strings[i+4].Trim()),
                    TwentyTwentyNine = double.Parse(strings[i+5].Trim()),
                    ThirtyThirtyNine = double.Parse(strings[i+6].Trim()),
                    FourtyFourtyNine = double.Parse(strings[i+7].Trim()),
                    Fifty = double.Parse(strings[i+8].Trim()),
                    ExtraPoints = double.Parse(strings[i+9].Trim()),
                    ExtraPointAttempts = double.Parse(strings[i+10].Trim()),
                    Games = double.Parse(strings[i+11].Trim())
                };
                players.Add(parse);
            }
            return players;
        }
        public async Task<int> DownloadHeadShots(string position)
        {
            var web = new HtmlWeb();
            List<string> urls = [];
            var count = 0;
            var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var activePlayers = (await playersService.GetAllPlayers()).Where(p => p.Active == 1 && p.Position == position).ToList();
            foreach (var player in activePlayers)
            {
                logger.Information("Getting url for {p}", player.Name);
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
                        var client = clientFactory.CreateClient();
                        var res = await client.GetAsync(url);
                        byte[] bytes = await res.Content.ReadAsByteArrayAsync();
                        MemoryStream memoryStream = new(bytes);
                        var fileName = @"C:\NFLData\Headshots\" + player.PlayerId.ToString() + ".png";
                        if (!File.Exists(fileName))
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
        public async Task<int> DownloadTeamLogos()
        {
            var teams = await playersService.GetAllTeams();
            var count = 0;
            foreach (var team in teams)
            {
                var url = _scraping.NFLTeamLogoURL + team.Team;
                var client = clientFactory.CreateClient();
                var res = await client.GetAsync(url);
                byte[] bytes = await res.Content.ReadAsByteArrayAsync();
                MemoryStream memoryStream = new(bytes);
                var fileName = @"C:\NFLData\Logos\" + team.Team.ToString() + ".png";
                if (!File.Exists(fileName))
                {
                    using var fs = new FileStream(fileName, FileMode.Create);
                    memoryStream.WriteTo(fs);
                    count++;
                }
            }
            return count;
        }
        public List<PlayerTeam> ParseFantasyProsPlayerTeam(string[] strings, string position)
        {
            var len = GetFantasyProsTableLength(position);
            List<PlayerTeam> playerTeams = [];
            for (int i = 0; i < strings.Length - len; i += len)
            {
                int start = strings[i].IndexOf("(") + 1;
                int end = strings[i].IndexOf(")", start);

                playerTeams.Add(new PlayerTeam
                {
                    Name = FormatName(strings[i]),
                    Team = strings[i][start..end]
                });
            }
            return playerTeams;
        }
        public async Task<List<Schedule>> ParseFantasyProsSeasonSchedule(string[] str)
        {
            List<List<string>> games = [];
            List<Schedule> schedules = [];

            foreach (var s in str)
            {
                games.Add([.. s.Split(homeSeparator, StringSplitOptions.RemoveEmptyEntries)]);
            }
            foreach (var g in games)
            {
                for (int i = 0; i < g.Count; i++)
                {
                    g[i] = g[i].Trim();
                    if (g.ElementAt(i).StartsWith("BYE"))
                    {
                        g.Insert(i - 1, "BYE");
                        g[i] = Regex.Replace(g.ElementAt(i), "BYE", string.Empty).Trim();
                        break;
                    }
                    else if (g.ElementAt(i).EndsWith("BYE"))
                    {
                        g.Insert(i + 1, "BYE");
                        g[i] = Regex.Replace(g.ElementAt(i), "BYE", string.Empty).Trim();
                        break;
                    }
                }
            }
            foreach (var g in games)
            {
                for (int i = 1; i < g.Count; i++)
                {
                    schedules.Add(new Schedule
                    {
                        Season = _season.CurrentSeason,
                        TeamId = await playersService.GetTeamId(g.ElementAt(0).Trim()),
                        Team = g.ElementAt(0).Trim(),
                        Week = i,
                        OpposingTeamId = g.ElementAt(i).Trim() == "BYE" ? 0 : await playersService.GetTeamId(g.ElementAt(i).Trim()),
                        OpposingTeam = g.ElementAt(i).Trim()
                    });
                }
            }
            return schedules;
        }
        public async Task<List<ProFootballReferenceGameScores>> ScrapeGameScores(int week)
        {
            var t = await Task.Run(() =>
            {
                List<ProFootballReferenceGameScores> scores = [];
                var url = string.Format("{0}{1}games.htm", "https://www.pro-football-reference.com/years/", _season.CurrentSeason.ToString() + "/");
                var xpath = "//*[@id=\"games\"]/tbody";
                var web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                var tempdata = doc.DocumentNode.SelectNodes(xpath)[0].InnerHtml;
                var strings = tempdata.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                strings = strings.Where(s => !s.Contains("aria-label") && s.Contains("data-stat")).ToList();
                List<List<string>> splits = [];
                foreach (var s in strings)
                {
                    var splitS = s.Split(dataSeparator, StringSplitOptions.RemoveEmptyEntries);
                    splits.Add([.. splitS]);
                }

                foreach (var split in splits)
                {
                    for (int index = split.Count - 1; index >= 0; index--)
                    {

                        if (split[index].Contains("<tr><th scope=\"row\" class=\"right"))
                        {
                            split.Remove(split[index]);
                        }
                        else
                        {
                            doc.LoadHtml(split[index]);
                            split[index] = doc.DocumentNode.InnerText;
                            var ind = split[index].IndexOf(">");
                            if (ind > -1)
                            {
                                split[index] = split[index][ind..].Trim();
                            }
                            split[index] = Regex.Replace(split[index], ">", string.Empty);
                        }
                    }
                    if (int.Parse(split[0]) == week)
                    {
                        scores.Add(new ProFootballReferenceGameScores
                        {
                            Week = int.Parse(split[0]),
                            Day = split[1],
                            Date = split[2],
                            Time = split[3],
                            Winner = split[4],
                            HomeIndicator = split[5],
                            Loser = split[6],
                            WinnerPoints = int.TryParse(split[8], out var wp) ? wp : 0,
                            LoserPoints = int.TryParse(split[9], out var lp) ? lp : 0,
                            WinnerYards = int.TryParse(split[10], out var wy) ? wy : 0,
                            LoserYards = int.TryParse(split[12], out var ly) ? ly : 0
                        });
                    }
                }
                return scores;
            });
            return t;
        }
        public async Task<List<FantasyProsADP>> ScrapeADP(string position)
        {
            var t = await Task.Run(() =>
            {
                var url = string.Format("{0}{1}.php", "https://www.fantasypros.com/nfl/adp/", position.Trim().ToLower());
                var xpath = "//*[@id=\"data\"]/tbody";
                var data = ScrapeData(url, xpath);
                var colCount = position.Trim().ToLower() == "qb" ? 9 : 7;
                List<FantasyProsADP> adp = [];
                for (int i = 0; i < data.Length - colCount; i += colCount)
                {
                    adp.Add(new FantasyProsADP
                    {
                        PositionADP = double.TryParse(data[i], out var pAdp) ? pAdp : 999,
                        OverallADP = double.TryParse(data[i + 1], out var oAdp) ? oAdp : 999,
                        Name = FormatName(data[i + 2])
                    });
                }
                foreach (var a in adp)
                {
                    if (a.Name.LastIndexOf(" ") > 1)
                    {
                        a.Name = a.Name[0..a.Name.LastIndexOf(" ")];
                    }
                }
                return adp;
            });
            return t;
        }

        private static string FormatName(string name) => Regex.Replace(Regex.Replace(name, @"[\d-]", string.Empty), @"\(.*\)", string.Empty).Trim();

        private static int GetFantasyProsTableLength(string position)
        {
            return position.ToUpper() switch{

                "QB" => 16,
                "RB" => 16,
                "WR" => 15,
                "TE" => 15,
                "K" => 15,
                _ => 0
            };
        }
    }
}
