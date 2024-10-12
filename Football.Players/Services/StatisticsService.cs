using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Football.Players.Services
{
    public class StatisticsService(IStatisticsRepository statisticsRepository, IPlayersService playersService, ISettingsService settingsService, IOptionsMonitor<Season> season) : IStatisticsService
    {
        private readonly Season _season = season.CurrentValue;
        public async Task<List<T>> GetSeasonData<T>(Position position, int queryParam, bool isPlayer) => await statisticsRepository.GetSeasonData<T>(position, queryParam, isPlayer);
        public async Task<List<T>> GetWeeklyData<T>(Position position, int playerId) => await statisticsRepository.GetWeeklyDataByPlayer<T>(position, playerId, _season.CurrentSeason);
        public async Task<List<T>> GetWeeklyData<T>(Position position, int season, int week) => await statisticsRepository.GetWeeklyData<T>(position, season, week);
        public async Task<List<T>> GetWeeklyDataByPlayer<T>(Position position, int playerId, int season) => await statisticsRepository.GetWeeklyDataByPlayer<T>(position, playerId, season);
        public async Task<List<T>> GetAllWeeklyDataByPosition<T>(Position position) => await statisticsRepository.GetAllWeeklyDataByPosition<T>(position);
        public async Task<List<T>> GetAllSeasonDataByPosition<T>(Position position) => (await statisticsRepository.GetAllSeasonDataByPosition<T>(position));
        public async Task<List<GameResult>> GetGameResults(int season) => await statisticsRepository.GetGameResults(season);
        public async Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week) => await statisticsRepository.GetWeeklyRosterPercentages(season, week);
        public async Task<List<SnapCount>> GetSnapCounts(int playerId) => await statisticsRepository.GetSnapCounts(playerId, _season.CurrentSeason);
        public async Task<IEnumerable<SnapCount>> GetSnapCountsBySeason(IEnumerable<int> playerIds, int season) => await statisticsRepository.GetSnapCountsBySeason(playerIds, season);
        public async Task<double> GetSnapsByGame(int playerId, int season, int week) => await statisticsRepository.GetSnapsByGame(playerId, season, week);
        public async Task<List<T>> GetSeasonDataByTeamIdAndPosition<T>(int teamId, Position position, int season) => await statisticsRepository.GetSeasonDataByTeamIdAndPosition<T>(teamId, position, season);
        public async Task<double> GetYearsExperience(int playerId, Position position) => await statisticsRepository.GetYearsExperience(playerId, position);
        public async Task<IEnumerable<StarterMissedGames>> GetCurrentStartersThatMissedGamesLastSeason(int currentSeason, int previousSeason, int maxGames, double avgProjection) => await statisticsRepository.GetCurrentStartersThatMissedGamesLastSeason(currentSeason, previousSeason, maxGames, avgProjection);
        public async Task<IEnumerable<SeasonADP>> GetAdpByPosition(int season, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.GetAdpByPosition(season);

            return await statisticsRepository.GetAdpByPosition(season, position.ToString());
        }

        public async Task<bool> DeleteAdpByPosition(int season, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.DeleteAdpByPosition(season);

            return await statisticsRepository.DeleteAdpByPosition(season, position.ToString());
        }

        public async Task<IEnumerable<ConsensusProjections>> GetConsensusProjectionsByPosition(int season, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.GetConsensusProjectionsByPosition(season);

            return await statisticsRepository.GetConsensusProjectionsByPosition(season, position.ToString());
        }

        public async Task<bool> DeleteConsensusProjectionsByPosition(int season, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.DeleteConsensusProjectionsByPosition(season);

            return await statisticsRepository.DeleteConsensusProjectionsByPosition(season, position.ToString());
        }

        public async Task<IEnumerable<ConsensusWeeklyProjections>> GetConsensusWeeklyProjectionsByPosition(int season, int week, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.GetConsensusWeeklyProjectionsByPosition(season, week);

            return await statisticsRepository.GetConsensusWeeklyProjectionsByPosition(season, week, position.ToString());
        }

        public async Task<bool> DeleteConsensusWeeklyProjectionsByPosition(int season, int week, Position position)
        {
            if (position == Position.FLEX) return await statisticsRepository.DeleteConsensusWeeklyProjectionsByPosition(season, week);

            return await statisticsRepository.DeleteConsensusWeeklyProjectionsByPosition(season, week, position.ToString());
        }
        public async Task<List<TeamRecord>> GetTeamRecords(int season)
        {
            var gameResults = await GetGameResults(season);
            var teams = await playersService.GetAllTeams();
            var currentWeek = await playersService.GetCurrentWeek(season);
            return teams.Select(team => new TeamRecord
            {
                TeamMap = team,
                Season = season,
                CurrentWeek = currentWeek,
                Wins = gameResults.Count(gr => gr.WinnerId == team.TeamId),
                Losses = gameResults.Count(gr => gr.LoserId == team.TeamId),
                Ties = gameResults.Count(gr => gr.LoserPoints == gr.WinnerPoints
                                            && (gr.HomeTeamId == team.TeamId || gr.AwayTeamId == team.TeamId))
            }).ToList();
        }

        public async Task<TeamRecord> GetTeamRecordInDivision(int teamId)
        {
            var gameResults = await GetGameResults(_season.CurrentSeason);
            var teamsInDivision = (await playersService.GetTeamsInDivision(teamId)).Select(t => t.TeamId).ToList();
            var gamesWon = gameResults.Where(g => (g.WinnerId == teamId && teamsInDivision.Contains(g.LoserId)));
            var gamesLost = gameResults.Where(g => (g.LoserId == teamId && teamsInDivision.Contains(g.WinnerId)));
            var gamesTied = gameResults.Where(g => (g.LoserPoints == g.WinnerPoints && ((g.HomeTeamId == teamId || g.AwayTeamId == teamId)) && (teamsInDivision.Contains(g.HomeTeamId) || teamsInDivision.Contains(g.AwayTeamId))));
            return new TeamRecord
            {
                Wins = gamesWon.Count(),
                Losses = gamesLost.Count(),
                Ties = gamesTied.Count()
            };
        }
        public async Task<List<T>> GetWeeklyData<T>(Position position, int playerId, string team)
        {
            var weeklyData = await GetWeeklyDataByPlayer<T>(position, playerId, _season.CurrentSeason);
            var teamChanges = await playersService.GetInSeasonTeamChanges();
            if (teamChanges.Any(t => t.PlayerId == playerId))
            {
                List<T> filteredData = [];
                var teamChange = teamChanges.First(t => t.PlayerId == playerId);

                foreach (var data in weeklyData)
                {
                    var week = (int)settingsService.GetValueFromModel(data, Model.Week);
                    if (teamChange.PreviousTeam == team && week < teamChange.WeekEffective)
                        filteredData.Add(data);
                    else if (teamChange.NewTeam == team && week >= teamChange.WeekEffective)
                        filteredData.Add(data);
                }
                return filteredData;
            }
            return weeklyData;
        }

        public async Task<double> GetAverageGamesMissed(int playerId)
        {
            var minGames = (await settingsService.GetSeasonTunings(_season.CurrentSeason)).MinGamesForMissedAverage;
            var player = await playersService.GetPlayer(playerId);
            _ = Enum.TryParse(player.Position, out Position position);
            var gamesPerSeason = await statisticsRepository.GetGamesPerSeason(playerId, position, minGames);

            var diff = 0.0;

            foreach (var gs in gamesPerSeason)
            {
                diff += await playersService.GetGamesBySeason(gs.Season) - gs.Games;
            }

            return gamesPerSeason.Any() ? diff / gamesPerSeason.Count() : 0;
        }

        public async Task<IEnumerable<DivisionStanding>> GetStandingsByDivision(Division division)
        {
            var teams = await playersService.GetTeamsByDivision(division);
            var teamMapDictionary = (await playersService.GetAllTeams()).ToDictionary(t => t.TeamId, t => t.TeamDescription);
            var conferenceTeams = await playersService.GetTeamsByConference(Enum.Parse<Conference>(teams.First().Conference));

            var allTeamRecords = await GetTeamRecords(_season.CurrentSeason);
            var gameResults = await GetGameResults(_season.CurrentSeason);
            List<TeamRecord> divisionalRecords = [];
            foreach (var team in teams)
            {
                divisionalRecords.Add(await GetTeamRecordInDivision(team.TeamId));
            }

            var winPercentages = CalculateWinPercentages(allTeamRecords, divisionalRecords);
            var teamsWithSameOverallWinPercentage = winPercentages.GroupBy(w => w.Value.Item1).Where(g => g.Count() > 1).Select(g => g.ToList());
            var orderedPercentages = winPercentages.OrderByDescending(w => w.Value.Item1).ToList();
            var divisionStandings = orderedPercentages
                                    .Select(w => new DivisionStanding
                                    {
                                        Division = division.ToString(),
                                        TeamId = w.Key,
                                        TeamDescription = teamMapDictionary[w.Key],
                                        Standing = orderedPercentages.IndexOf(w)
                                    });

            var sameWinCount = teamsWithSameOverallWinPercentage.Count();
            if (sameWinCount == 0) return divisionStandings;

            foreach (var winGroup in teamsWithSameOverallWinPercentage)
            {
                Dictionary<int, List<(int, double?)>> groupHeadToHeadDictionary = [];
                for (int i = 0; i < winGroup.Count; i++)
                {
                    var teamId = winGroup.ElementAt(i).Key;
                    var h2hs = GetHeadToHeads(teamId, gameResults, winGroup.Where(t => t.Key != teamId).Select(t => t.Key));
                }

            }


            return [];
        }

        private Dictionary<int, (double, double)> CalculateWinPercentages(List<TeamRecord> allTeamRecords, List<TeamRecord> divisionalRecords)
        {
            var join = allTeamRecords.Join(divisionalRecords, a => a.TeamMap.TeamId, d => d.TeamMap.TeamId, (a, d) => new { a.TeamMap.TeamId, OverallRecord = a, DivisionalRecord = d });
            Dictionary<int, (double, double)> winPercentages = [];

            foreach (var j in join)
            {
                var overallWinPercentage = j.OverallRecord.Wins / (j.OverallRecord.Wins + j.OverallRecord.Losses + j.OverallRecord.Ties);
                var divisionalWinPercentage = j.DivisionalRecord.Wins / (j.DivisionalRecord.Wins + j.DivisionalRecord.Losses + j.DivisionalRecord.Ties);
                winPercentages.Add(j.TeamId, (overallWinPercentage, divisionalWinPercentage));
            }
            return winPercentages;
        }

        private List<(int, double?)> GetHeadToHeads(int teamId, List<GameResult> gameResults, IEnumerable<int> otherTeamIds)
        {
            List<(int, double?)> h2hs = [];
            foreach (var otherId in otherTeamIds)
            {
                var h2hWins = gameResults.Where(g => g.WinnerId == teamId && g.LoserId == otherId);
                var total = gameResults.Where(g => (g.HomeTeamId == teamId && g.AwayTeamId == otherId) || (g.AwayTeamId == teamId && g.HomeTeamId == otherId));
                if (total.Any()) h2hs.Add((otherId, h2hWins.Count() / total.Count()));
                else h2hs.Add((otherId, null));

            }
            return h2hs;
        }
    }
}
