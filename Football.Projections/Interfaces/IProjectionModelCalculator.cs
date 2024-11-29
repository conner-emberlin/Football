using AutoMapper;
using Football.Enums;
using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Models;
using Football.Projections.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IProjectionModelCalculator
    {
        Task<List<WeeklyFantasy>> WeeklyFantasyProjectionModel(Position position);
        Task<List<QBModelWeek>> QBWeeklyProjectionModel();
        Task<List<RBModelWeek>> RBWeeklyProjectionModel();
        Task<List<WRModelWeek>> WRWeeklyProjectionModel();
        Task<List<TEModelWeek>> TEWeeklyProjectionModel();
        Task<List<KModelWeek>> KWeeklyProjectionModel();
        Task<List<DSTModelWeek>> DSTWeeklyProjectionModel();
        Task<Vector<double>> GetWeeklyWeightedAverageVector(Player player, int currentWeek, WeeklyTunings tunings, List<string>? filter = null, bool neuralNetwork = false);
        Task<List<SeasonFantasy>> SeasonFantasyProjectionModel(Position position, int gameTrim);
        Task<List<QBModelSeason>> QBSeasonProjectionModel(int gameTrim);
        Task<List<RBModelSeason>> RBSeasonProjectionModel(int gameTrim);
        Task<List<WRModelSeason>> WRSeasonProjectionModel(int gameTrim);
        Task<List<TEModelSeason>> TESeasonProjectionModel(int gameTrim);
        Task<Vector<double>> GetSeasonWeightedAverageVector(Player player, double gamesPlayedInjured, Tunings tunings, int seasonGames, List<string>? filter = null);
    }
}
