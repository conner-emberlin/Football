using AutoMapper;
using Football.Enums;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Services
{
    public class ProjectionModelCalculator(IFantasyDataService fantasyService, IStatisticsService statisticsService, IStatProjectionCalculator statCalculator, IMatrixCalculator matrixCalculator, IMapper mapper) : IProjectionModelCalculator
    {
        public Task<List<WeeklyFantasy>> WeeklyFantasyProjectionModel(Position position) => fantasyService.GetAllWeeklyFantasyByPosition(position);
        public async Task<List<QBModelWeek>> QBWeeklyProjectionModel() => mapper.Map<List<QBModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataQB>(Position.QB));
        public async Task<List<RBModelWeek>> RBWeeklyProjectionModel() => mapper.Map<List<RBModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataRB>(Position.RB));
        public async Task<List<WRModelWeek>> WRWeeklyProjectionModel() => mapper.Map<List<WRModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataWR>(Position.WR));
        public async Task<List<TEModelWeek>> TEWeeklyProjectionModel() => mapper.Map<List<TEModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataTE>(Position.TE));
        public async Task<List<KModelWeek>> KWeeklyProjectionModel() => mapper.Map<List<KModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataK>(Position.K));
        public async Task<List<DSTModelWeek>> DSTWeeklyProjectionModel() => mapper.Map<List<DSTModelWeek>>(await statisticsService.GetAllWeeklyDataByPosition<AllWeeklyDataDST>(Position.DST));
        public async Task<Vector<double>> GetWeeklyWeightedAverageVector(Player player, int currentWeek, WeeklyTunings tunings, List<string>? filter = null)
        {
            _ = Enum.TryParse(player.Position, out Position position);

            switch (position)
            {
                case Position.QB:
                    var modelQB = mapper.Map<QBModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, player.PlayerId), currentWeek, tunings));
                    modelQB.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek, tunings)).Snaps;
                    return matrixCalculator.TransformModel(modelQB, filter);
                case Position.RB:
                    var modelRB = mapper.Map<RBModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataRB>(Position.RB, player.PlayerId), currentWeek, tunings));
                    modelRB.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek, tunings)).Snaps;
                    return matrixCalculator.TransformModel(modelRB, filter);
                case Position.WR:
                    var modelWR = mapper.Map<WRModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataWR>(Position.WR, player.PlayerId), currentWeek, tunings));
                    modelWR.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek, tunings)).Snaps;
                    return matrixCalculator.TransformModel(modelWR, filter);
                case Position.TE:
                    var modelTE = mapper.Map<TEModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataTE>(Position.TE, player.PlayerId), currentWeek, tunings));
                    modelTE.SnapsPerGame = (statCalculator.WeightedWeeklyAverage(await statisticsService.GetSnapCounts(player.PlayerId), currentWeek, tunings)).Snaps;
                    return matrixCalculator.TransformModel(modelTE, filter);
                case Position.K:
                    var modelK = mapper.Map<KModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataK>(Position.K, player.PlayerId), currentWeek, tunings));
                    return matrixCalculator.TransformModel(modelK, filter);
                case Position.DST:
                    var modelDST = mapper.Map<DSTModelWeek>(statCalculator.WeightedWeeklyAverage(await statisticsService.GetWeeklyData<WeeklyDataDST>(Position.DST, player.PlayerId), currentWeek, tunings));
                    return matrixCalculator.TransformModel(modelDST, filter);
                default: return Vector<double>.Build.Dense(0);
            }
        }

        public async Task<List<SeasonFantasy>> SeasonFantasyProjectionModel(Position position, int gameTrim)
        {
            var seasonFantasy = await fantasyService.GetAllSeasonFantasyByPosition(position, gameTrim);
            return seasonFantasy;
        }

        public async Task<List<QBModelSeason>> QBSeasonProjectionModel(int gameTrim)
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataQB>(Position.QB);
            return mapper.Map<List<QBModelSeason>>(seasonData.Where(s => s.Games >= gameTrim));
        }

        public async Task<List<RBModelSeason>> RBSeasonProjectionModel(int gameTrim)
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataRB>(Position.RB);
            return mapper.Map<List<RBModelSeason>>(seasonData.Where(s => s.Games >= gameTrim));
        }

        public async Task<List<WRModelSeason>> WRSeasonProjectionModel(int gameTrim)
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataWR>(Position.WR);
            return mapper.Map<List<WRModelSeason>>(seasonData.Where(s => s.Games >= gameTrim));
        }

        public async Task<List<TEModelSeason>> TESeasonProjectionModel(int gameTrim)
        {
            var seasonData = await statisticsService.GetAllSeasonDataByPosition<AllSeasonDataTE>(Position.TE);
            return mapper.Map<List<TEModelSeason>>(seasonData.Where(s => s.Games >= gameTrim));
        }
        public async Task<Vector<double>> GetSeasonWeightedAverageVector(Player player, double gamesPlayedInjured, Tunings tunings, int seasonGames, List<string>? filter = null)
        {
            _ = Enum.TryParse(player.Position, out Position position);

            switch (position)
            {
                case Position.QB:
                    var modelQB = mapper.Map<QBModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataQB>(Position.QB, player.PlayerId, true), gamesPlayedInjured, tunings, seasonGames));
                    modelQB.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.QB);
                    return matrixCalculator.TransformModel(modelQB, filter);
                case Position.RB:
                    var modelRB = mapper.Map<RBModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataRB>(Position.RB, player.PlayerId, true), gamesPlayedInjured, tunings, seasonGames));
                    modelRB.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.RB);
                    return matrixCalculator.TransformModel(modelRB, filter);
                case Position.WR:
                    var modelWR = mapper.Map<WRModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataWR>(Position.WR, player.PlayerId, true), gamesPlayedInjured, tunings, seasonGames));
                    modelWR.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.WR);
                    return matrixCalculator.TransformModel(modelWR, filter);
                case Position.TE:
                    var modelTE = mapper.Map<TEModelSeason>(statCalculator.CalculateStatProjection(await statisticsService.GetSeasonData<SeasonDataTE>(Position.TE, player.PlayerId, true), gamesPlayedInjured, tunings, seasonGames));
                    modelTE.YearsExperience = await statisticsService.GetYearsExperience(player.PlayerId, Position.TE);
                    return matrixCalculator.TransformModel(modelTE, filter);
                default: return Vector<double>.Build.Dense(0);

            }
        }
    }
}
