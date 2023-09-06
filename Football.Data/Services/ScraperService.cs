using Football.Data.Interfaces;
using Football.Data.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Football.Data.Services
{
    public class ScraperService : IScraperService
    {
        private readonly WeeklyScraping _scraping;
        public ScraperService(IOptionsMonitor<WeeklyScraping> scraping)
        {
            _scraping = scraping.CurrentValue;
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


        private string FormatName(string name)
        {
            name = Regex.Replace(name, @"[\d-]", string.Empty);
            name = Regex.Replace(name, @"\(.*\)", string.Empty).Trim();
            return name;
        }
    }
}
